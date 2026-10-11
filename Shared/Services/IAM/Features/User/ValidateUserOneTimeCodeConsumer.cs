using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Services;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateUserOneTimeCodeConsumer(UserManager<ApplicationUserEntity> userManager,
                                             TotpService totpService) : IConsumer<ValidateUserOneTimeCode>
{
    public async Task Consume(ConsumeContext<ValidateUserOneTimeCode> context)
    {
        var user = await userManager.FindByEmailAsync(userManager.NormalizeEmail(context.Message.Email));
        if (user is not default(ApplicationUserEntity))
        {
            if (string.IsNullOrWhiteSpace(user.TotpSecret))
            {
                await context.RespondAsync(
                    new DataResult<UserLoginResult>(
                        Errors: [new(nameof(user.TotpSecret), UserConstants.UserTotpNotConfiguredError)]));
                return;
            }

            if (!totpService.Verify(user.TotpSecret, context.Message.Code))
            {
                await context.RespondAsync(
                    new DataResult<UserLoginResult>(
                        Errors: [new(nameof(context.Message.Code), UserConstants.UserTotpCodeVerificationError)]));
                return;
            }

            _ = Guid.TryParse(user.Id, out var userId);

            await context.RespondAsync(
                new DataResult<UserLoginResult>(new UserLoginResult { Id = userId }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(user), UserConstants.UserNotFoundError)]));
        }
    }
}
