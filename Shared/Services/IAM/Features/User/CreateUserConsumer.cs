using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;
using snowcoreBlog.Backend.IAM.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class CreateUserConsumer(IValidator<CreateUser> validator,
                                IPasswordHasher passwordHasher,
                                IUserStore<ApplicationUser> userStore) : IConsumer<CreateUser>
{
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var result = await validator.ValidateAsync(context.Message);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserCreationResult>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var passwordHash = passwordHasher.HashPassword(context.Message.Password);

        var userEntity = context.Message.ToEntity(passwordHash);
        var creationResult = await userStore.CreateAsync(userEntity, context.CancellationToken);
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