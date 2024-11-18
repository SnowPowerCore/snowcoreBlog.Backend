using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateUserExistsConsumer(IUserStore<ApplicationUser> userStore) : IConsumer<ValidateUserExists>
{
    public async Task Consume(ConsumeContext<ValidateUserExists> context)
    {
        var user = await ((IUserEmailStore<ApplicationUser>)userStore).FindByEmailAsync(
            context.Message.Email.ToUpper(), context.CancellationToken);
        if (user is not default(ApplicationUser))
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