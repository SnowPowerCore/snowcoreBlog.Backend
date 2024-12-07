using MassTransit;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateTempUserExistsConsumer(IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<ValidateTempUserExists>
{
    public async Task Consume(ConsumeContext<ValidateTempUserExists> context)
    {
        var userExists = await applicationTempUserRepository.CheckTempUserExistsByEmailAsync(context.Message.Email);
        if (userExists)
        {
            await context.RespondAsync(
                new DataResult<TempUserExistsValidationResult>(new() { Exists = true }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<TempUserExistsValidationResult>(new()));
        }
    }
}