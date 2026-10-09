using MassTransit;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Services;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class SetupUserAuthenticatorConsumer(IHasher hasher,
                                            TotpService totpService,
                                            IOptions<ValidStates<HashedStringsVerificationResult>> validStatesOptions,
                                            IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<SetupUserAuthenticator>
{
    public async Task Consume(ConsumeContext<SetupUserAuthenticator> context)
    {
        var tempUserEntity = await applicationTempUserRepository
            .GetTempUserByEmailAsync(context.Message.Email, context.CancellationToken);
        if (tempUserEntity is default(ApplicationTempUserEntity))
        {
            await context.RespondAsync(
                new DataResult<AuthenticatorSetupResult>(
                    Errors: [new(nameof(tempUserEntity), TempUserConstants.TempUserNotFoundError)]));
            return;
        }

        if (!validStatesOptions.Value.States.Contains(
                hasher.VerifyHashedStrings(tempUserEntity.ActivationToken, context.Message.TempUserVerificationToken)))
        {
            await context.RespondAsync(
                new DataResult<AuthenticatorSetupResult>(
                    Errors: [new(nameof(context.Message.TempUserVerificationToken), UserConstants.UserTokenVerificationError)]));
            return;
        }

        if (tempUserEntity.ActivationTokenExpirationDate <= DateTimeOffset.UtcNow)
        {
            await context.RespondAsync(
                new DataResult<AuthenticatorSetupResult>(
                    Errors: [new(nameof(tempUserEntity.ActivationTokenExpirationDate), UserConstants.UserTokenVerificationError)]));
            return;
        }

        var secret = totpService.GenerateSecret();

        await applicationTempUserRepository.AddOrUpdateAsync(
            tempUserEntity with { TotpSecret = secret }, token: context.CancellationToken);

        await context.RespondAsync(
            new DataResult<AuthenticatorSetupResult>(new AuthenticatorSetupResult
            {
                Secret = secret,
                OtpAuthUri = totpService.BuildOtpAuthUri(secret, tempUserEntity.Email)
            }));
    }
}
