using Fido2NetLib;
using Fido2NetLib.Objects;
using FluentValidation;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateAndCreateUserConsumer(IHasher hasher,
                                           IFido2 fido2,
                                           IValidator<CreateUser> validator,
                                           IOptions<ValidStates<HashedStringsVerificationResult>> validStatesOptions,
                                           UserManager<ApplicationUserEntity> userManager,
                                           IFido2PublicKeyCredentialRepository fido2PublicKeyCredentialRepository,
                                           IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<CreateUser>
{
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var tempUserEntity = await applicationTempUserRepository
            .GetTempUserByEmailAsync(context.Message.Email, context.CancellationToken);
        if (tempUserEntity is not default(ApplicationTempUserEntity))
        {
            if (!validStatesOptions.Value.States.Contains(
                hasher.VerifyHashedStrings(tempUserEntity.ActivationToken, context.Message.TempUserVerificationToken)))
            {
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: [new(nameof(context.Message.TempUserVerificationToken), UserConstants.UserTokenVerificationError)]));
                return;
            }

            if (tempUserEntity.ActivationTokenExpirationDate <= DateTimeOffset.UtcNow)
            {
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: [new(nameof(tempUserEntity.ActivationTokenExpirationDate), UserConstants.UserTokenVerificationError)]));
                return;
            }

            var options = CredentialCreateOptions.FromJson(context.Message.AttestationOptionsJson);

            async Task<bool> IsCredentialIdUniqueToUserAsync(IsCredentialIdUniqueToUserParams @params, CancellationToken cancellationToken)
            {
                var userId = new Guid(@params.User.Id).ToString();
                var creds = await userManager.Users
                    .Where(user => user.Id == userId)
                    .SelectMany(user => user.PublicKeyCredentials)
                    .ToListAsync(context.CancellationToken);
                return !await fido2PublicKeyCredentialRepository
                    .CheckPublicKeyCredExistsAsync(creds.ToArray(), @params.CredentialId, cancellationToken);
            }

            var credentialResult = await fido2.MakeNewCredentialAsync(
                context.Message.AuthenticatorAttestation.ToMakeNewCredentialParams(
                    options,
                    IsCredentialIdUniqueToUserAsync),
                context.CancellationToken);

            if (credentialResult is default(RegisteredPublicKeyCredential))
            {
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: [new(nameof(credentialResult), UserConstants.CredentialsVerificationError)]));
                return;
            }

            var userId = new Guid(credentialResult.User.Id);

            var credential = new Fido2PublicKeyCredentialEntity
            {
                Id = Guid.CreateVersion7(),
                PublicKeyCredentialId = credentialResult.Id.ToList(),
                PublicKey = credentialResult.PublicKey.ToList(),
                SignatureCounter = credentialResult.SignCount,
                IsBackupEligible = credentialResult.IsBackupEligible,
                IsBackedUp = credentialResult.IsBackedUp,
                AttestationObject = credentialResult.AttestationObject.ToList(),
                AttestationClientDataJson = credentialResult.AttestationClientDataJson.ToList(),
                AttestationFormat = credentialResult.AttestationFormat,
                AaGuid = credentialResult.AaGuid,
                UserId = userId
            };

            foreach (var authenticatorTransport in credentialResult.Transports)
            {
                credential.AuthenticatorTransports.Add(new Fido2AuthenticatorTransportEntity
                {
                    Id = Guid.CreateVersion7(),
                    PublicKeyId = credential.Id,
                    PublicKeyCredentialId = credentialResult.Id.ToList(),
                    Value = authenticatorTransport
                });
            }

            if (credentialResult.PublicKey is not null)
            {
                credential.DevicePublicKeys.Add(new Fido2DevicePublicKeyEntity
                {
                    Id = Guid.CreateVersion7(),
                    PublicKeyId = credential.Id,
                    PublicKeyCredentialId = credentialResult.Id.ToList(),
                    Value = credentialResult.PublicKey.ToList()
                });
            }

            var userEntity = tempUserEntity.ToUserEntity(userId, credential);
            var creationResult = await userManager.CreateAsync(userEntity);
            if (creationResult.Succeeded)
            {
                await applicationTempUserRepository.RemoveAsync(tempUserEntity, token: CancellationToken.None);
                await fido2PublicKeyCredentialRepository.AddOrUpdateAsync(credential, token: CancellationToken.None);
                
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(new UserCreationResult
                    {
                        Id = userId,
                        Email = userEntity.Email!,
                        UserName = userEntity.UserName!
                    }));
            }
            else
            {
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: creationResult.Errors.Select(e => new NoneDetail(e.Code, e.Description)).ToList()));
            }
        }
        else
        {
            await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: [new(nameof(tempUserEntity), TempUserConstants.TempUserNotFoundError)]));
        }
    }
}