using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateUserExistsConsumer(UserManager<ApplicationUserEntity> userManager) : IConsumer<ValidateUserExists>
{
    public async Task Consume(ConsumeContext<ValidateUserExists> context)
    {
        var user = await userManager.FindByEmailAsync(userManager.NormalizeEmail(context.Message.Email));
        if (user is not default(ApplicationUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserExistsValidationResult>(new() { Exists = true }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<UserExistsValidationResult>(new()));
        }
    }
}