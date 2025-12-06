using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Shared;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;

public class ValidateReaderAccountNotExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IMaybe<ReaderAccountCreatedDto>>
{
    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await ValidateReaderAccountNotExistsSharedStep.CheckAsync(
            requestClient,
            context.ConfirmRequest.ToValidateUserExists(),
            token);
        if (result is Some<bool> success && !success.Value)
        {
            return await next(context, token);
        }
        if (result is INone noneWithErrors)
        {
            return noneWithErrors.Cast<ReaderAccountCreatedDto>();
        }
        return ReaderAccountAlreadyExistsError<ReaderAccountCreatedDto>.Create(
            ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError);
    }
}