using FluentValidation;
using MassTransit;
using Results;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class CreateTempUserConsumer(IValidator<CreateTempUser> validator,
                                    IHasher hasher,
                                    IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<CreateTempUser>
{
    public async Task Consume(ConsumeContext<CreateTempUser> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<TempUserCreationResult>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var verificationToken = StringExtensions.RandomString(32);
        var verificationTokenExpirationDate = DateTimeOffset.UtcNow.AddDays(7);
        var tempUserEntity = context.Message.ToEntity(
            hasher.Hash(verificationToken),
            verificationTokenExpirationDate);
        var tempUserCreationResult = await applicationTempUserRepository.AddOrUpdateAsync(tempUserEntity);
        if (tempUserCreationResult is not default(ApplicationTempUserEntity))
        {
            await context.RespondAsync(
                new DataResult<TempUserCreationResult>(new TempUserCreationResult
                {
                    Id = tempUserCreationResult.Id,
                    FirstName = tempUserCreationResult.FirstName,
                    Email = tempUserCreationResult.Email!,
                    InitialEmailConsent = tempUserCreationResult.InitialEmailConsent,
                    VerificationToken = verificationToken,
                    VerificationTokenExpirationDate = tempUserCreationResult.ActivationTokenExpirationDate,
                }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<TempUserCreationResult>(
                    Errors: [new(nameof(CreateTempUser), TempUserConstants.TempUserUnableToCreateUpdateError)]));
        }
    }
}