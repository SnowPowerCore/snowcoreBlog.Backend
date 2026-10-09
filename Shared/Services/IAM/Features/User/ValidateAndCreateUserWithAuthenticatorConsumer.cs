using FluentValidation;
using MassTransit;
using MaybeResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Services;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateAndCreateUserWithAuthenticatorConsumer(IHasher hasher,
                                                            TotpService totpService,
                                                            IValidator<CreateUserWithAuthenticator> validator,
                                                            IOptions<ValidStates<HashedStringsVerificationResult>> validStatesOptions,
                                                            UserManager<ApplicationUserEntity> userManager,
                                                            IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<CreateUserWithAuthenticator>
{
    public async Task Consume(ConsumeContext<CreateUserWithAuthenticator> context)
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
        if (tempUserEntity is default(ApplicationTempUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: [new(nameof(tempUserEntity), TempUserConstants.TempUserNotFoundError)]));
            return;
        }

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

        if (string.IsNullOrWhiteSpace(tempUserEntity.TotpSecret) ||
            !totpService.Verify(tempUserEntity.TotpSecret, context.Message.Code))
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: [new(nameof(context.Message.Code), UserConstants.UserTotpCodeVerificationError)]));
            return;
        }

        var userId = Guid.CreateVersion7();

        var userEntity = tempUserEntity.ToUserEntity(userId);
        var creationResult = await userManager.CreateAsync(userEntity);
        if (creationResult.Succeeded)
        {
            await applicationTempUserRepository.RemoveAsync(tempUserEntity, token: CancellationToken.None);

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
}
