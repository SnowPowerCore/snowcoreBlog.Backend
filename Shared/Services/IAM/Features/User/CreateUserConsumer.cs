using FluentValidation;
using MassTransit;
using Results;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;
using snowcoreBlog.Backend.IAM.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Stores;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class CreateUserConsumer(IValidator<CreateUser> validator,
                                IPasswordHasher passwordHasher,
                                MartenUserStore<ApplicationUser> userStore) : IConsumer<CreateUser>
{
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var result = await validator.ValidateAsync(context.Message);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new ValidationErrorResult<UserCreationResult>(result));
        }

        var passwordHash = passwordHasher.HashPassword(context.Message.Password);

        var userEntity = context.Message.ToEntity(passwordHash);
        var creationResult = await userStore.CreateAsync(userEntity, context.CancellationToken);
        if (creationResult.Succeeded)
        {
            await context.RespondAsync(
                Result.Success(new UserCreationResult { Id = Guid.Parse(userEntity.Id) }));
        }
        else
        {
            await context.RespondAsync(
                new ApplicationIdentityStoreCreationError<UserCreationResult>(creationResult.Errors.ToList()));
        }
    }
}