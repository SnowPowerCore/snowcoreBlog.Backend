using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

public class CreateUserForReaderAccountStep(IRequestClient<CreateUser> client) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext>
{
    public async Task InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<IResult<UserCreationResult>>(context.Request.ToCreateUser());
        if (response.Message is SuccessResult<UserCreationResult> success)
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateUserForReaderAccountResult, success);
        }
        else if (response.Message is IErrorResult<UserCreationResult> error)
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(error.Message, error.Errors));
            return;
        }
        else
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.UserForReaderAccountCreationGenericError));
            return;
        }

        await next(context, token);
    }
}