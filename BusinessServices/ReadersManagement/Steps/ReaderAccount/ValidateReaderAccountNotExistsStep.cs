using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

public class ValidateReaderAccountNotExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext>
{
    public async Task InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(context.Request.ToValidateUserExists());
        if (result.Message.IsSuccess)
        {
            if (!result.Message.Value!.Exists)
            {
                await next(context, token);
                return;
            }
            else
            {
                context.SetDataWith(
                    ReaderAccountConstants.CreateReaderAccountResult,
                    ReaderAccountAlreadyExistsError<ReaderAccountCreationResultDto>.Create(
                        ReaderAccountConstants.ReaderAccountAlreadyExistsError));
            }
        }
        else
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError));
        }
    }
}