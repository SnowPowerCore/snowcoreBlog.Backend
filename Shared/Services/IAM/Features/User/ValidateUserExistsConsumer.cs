using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Entities;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateUserExistsConsumer(IUserEmailStore<ApplicationUser> userStore) : IConsumer<ValidateUserExists>
{
    public async Task Consume(ConsumeContext<ValidateUserExists> context)
    {
        var user = await userStore.FindByEmailAsync(context.Message.Email, context.CancellationToken);
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