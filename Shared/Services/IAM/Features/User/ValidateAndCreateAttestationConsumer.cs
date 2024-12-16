using Fido2NetLib;
using Fido2NetLib.Objects;
using MassTransit;
using Microsoft.Extensions.Options;
using Results;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.ErrorResults;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateAndCreateAttestationConsumer(IHasher hasher,
                                                  IFido2 fido2,
                                                  IOptions<ValidStates<HashedStringsVerificationResult>> validStatesOptions,
                                                  IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<ValidateAndCreateAttestation>
{
    public async Task Consume(ConsumeContext<ValidateAndCreateAttestation> context)
    {
        var tempUser = await applicationTempUserRepository.GetTempUserByEmailAsync(context.Message.Email, context.CancellationToken);
        if (tempUser is not default(ApplicationTempUserEntity))
        {
            var dateTimeNow = DateTime.UtcNow;
            if (validStatesOptions.Value.States.Contains(hasher.VerifyHashedStrings(tempUser.ActivationToken, context.Message.VerificationToken))
                && tempUser.ActivationTokenExpirationDate > dateTimeNow)
            {
                // As validation of the token succeeded, we expect that registration process won't take much longer,
                // so we change the token expiration date to be significantly closer: 5 mins from the current time.
                var updatedTempUser = await applicationTempUserRepository
                    .AddOrUpdateAsync(tempUser with { ActivationTokenExpirationDate = dateTimeNow.AddMinutes(5) }, tempUser.Id,
                        token: context.CancellationToken);

                var user = new Fido2User
                {
                    Name = updatedTempUser.Email,
                    Id = Guid.NewGuid().ToByteArray(),
                    DisplayName = !string.IsNullOrEmpty(updatedTempUser.LastName)
                        ? $"{updatedTempUser.FirstName} {updatedTempUser.LastName}"
                        : $"{updatedTempUser.FirstName}"
                };

                var authenticatorSelection = new AuthenticatorSelection
                {
                    AuthenticatorAttachment = context.Message.AuthenticatorAttachment,
                    ResidentKey = context.Message.ResidentKey,
                    UserVerification = context.Message.UserVerification
                };

                var attestationPreference = context.Message.AttestationType;

                var extensions = new AuthenticationExtensionsClientInputs
                {
                    Extensions = true,
                    UserVerificationMethod = true,
                    CredProps = true,
                };

                await context.RespondAsync(Result.Success(fido2.RequestNewCredential(
                    user.ToRequestNewCredentialParams(attestationPreference, authenticatorSelection, extensions))));
            }
            else
            {
                await context.RespondAsync(
                    ValidateTokenForAttestationError<CredentialCreateOptions>.Create(
                        TempUserConstants.TempUserTokenVerificationError));
            }
        }
        else
        {
            await context.RespondAsync(
                ValidateTokenForAttestationError<CredentialCreateOptions>.Create(
                    TempUserConstants.TempUserNotFoundError));
        }
    }
}