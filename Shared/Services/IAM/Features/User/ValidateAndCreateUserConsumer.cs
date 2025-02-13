using Fido2NetLib;
using Fido2NetLib.Objects;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Results;
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
                                           IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<CreateUser>
{
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
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

            Task<bool> IsCredentialIdUniqueToUserAsync(IsCredentialIdUniqueToUserParams @params, CancellationToken cancellationToken) =>
                userManager.Users
                    .SelectMany(user => user.PublicKeyCredentials)
                    .AllAsync(credential => credential.Id != new Guid(@params.CredentialId), context.CancellationToken);

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

            var credential = new Fido2PublicKeyCredentialEntity
            {
                Id = new Guid(credentialResult.Id),
                PublicKey = credentialResult.PublicKey,
                SignatureCounter = credentialResult.SignCount,
                IsBackupEligible = credentialResult.IsBackupEligible,
                IsBackedUp = credentialResult.IsBackedUp,
                AttestationObject = credentialResult.AttestationObject,
                AttestationClientDataJson = credentialResult.AttestationClientDataJson,
                AttestationFormat = credentialResult.AttestationFormat,
                AaGuid = credentialResult.AaGuid,
                UserId = new Guid(credentialResult.User.Id)
            };

            foreach (var authenticatorTransport in credentialResult.Transports)
            {
                credential.AuthenticatorTransports.Add(new Fido2AuthenticatorTransportEntity
                {
                    PublicKeyCredentialId = new Guid(credentialResult.Id),
                    Value = authenticatorTransport
                });
            }

            if (credentialResult.PublicKey is not null)
            {
                credential.DevicePublicKeys.Add(new Fido2DevicePublicKeyEntity
                {
                    PublicKeyCredentialId = new Guid(credentialResult.Id),
                    Value = credentialResult.PublicKey
                });
            }

            var userEntity = tempUserEntity.ToUserEntity(credential);
            var creationResult = await userManager.CreateAsync(userEntity);
            if (creationResult.Succeeded)
            {
                await applicationTempUserRepository.RemoveAsync(tempUserEntity, token: context.CancellationToken);

                await context.RespondAsync(
                    new DataResult<UserCreationResult>(new UserCreationResult
                    {
                        Id = Guid.Parse(userEntity.Id),
                        Email = userEntity.Email!,
                        UserName = userEntity.UserName!
                    }));
            }
            else
            {
                await context.RespondAsync(
                    new DataResult<UserCreationResult>(
                        Errors: creationResult.Errors.Select(e => new ErrorResultDetail(e.Code, e.Description)).ToList()));
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