using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Shared;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;

public class ValidateReaderAccountNotExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IMaybe<RequestReaderAccountCreationResultDto>>
{
    public async Task<IMaybe<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await ValidateReaderAccountNotExistsSharedStep.CheckAsync(
            requestClient,
            context.CreateRequest.ToValidateUserExists(),
            token);
        if (result is Some<bool> success && !success.Value)
        {
            return await next(context, token);
        }
        if (result is INone noneWithErrors)
        {
            return noneWithErrors.Cast<RequestReaderAccountCreationResultDto>();
        }
        return ReaderAccountAlreadyExistsError<RequestReaderAccountCreationResultDto>.Create(
            ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError);
    }
}