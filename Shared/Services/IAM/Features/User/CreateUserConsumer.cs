using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class CreateUserConsumer(IValidator<CreateUser> validator,
                                UserManager<ApplicationUser> userManager) : IConsumer<CreateUser>
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

        var userEntity = context.Message.ToEntity();
        var creationResult = await userManager.CreateAsync(userEntity);
        if (creationResult.Succeeded)
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(new UserCreationResult { Id = Guid.Parse(userEntity.Id) }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: creationResult.Errors.Select(e => new ErrorResultDetail(e.Code, e.Description)).ToList()));
        }
    }
}