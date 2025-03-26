using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;

public class ValidateReaderAccountTempRecordNotExistsStep(IRequestClient<ValidateTempUserExists> requestClient) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IMaybe<RequestReaderAccountCreationResultDto>>
{
    public async Task<IMaybe<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<TempUserExistsValidationResult>>(
            context.CreateRequest.ToValidateTempUserExists(), token);
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return ReaderAccountAlreadyExistsError<RequestReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountAlreadyExistsError);
            }
            else
            {
                return await next(context, token);
            }
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError, result.Message.Errors);
        }
    }
}