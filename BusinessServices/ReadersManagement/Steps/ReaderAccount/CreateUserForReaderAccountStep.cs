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
        var response = await client.GetResponse<DataResult<UserCreationResult>>(context.Request.ToCreateUser(), token);
        if (response.Message.IsSuccess)
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateUserForReaderAccountResult, Result.Success(response.Message.Value));
        }
        else
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.UserForReaderAccountUnableToCreateUpdateError, response.Message.Errors));
            return;
        }
        // else
        // {
        //     context.SetDataWith(
        //         ReaderAccountConstants.CreateReaderAccountResult,
        //         CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
        //             ReaderAccountConstants.UserForReaderAccountCreationGenericError));
        //     return;
        // }

        await next(context, token);
    }
}