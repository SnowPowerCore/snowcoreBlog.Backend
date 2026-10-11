using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Infrastructure.StepProgress;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Shared;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;

public class ValidateReaderAccountNotExistsForAuthenticatorStep(IRequestClient<ValidateUserExists> requestClient) : IStep<ConfirmCreateReaderAccountWithAuthenticatorDelegate, ConfirmCreateReaderAccountWithAuthenticatorContext, IMaybe<ReaderAccountCreatedDto>>
{
    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountWithAuthenticatorContext context, ConfirmCreateReaderAccountWithAuthenticatorDelegate next, CancellationToken token = default)
    {
        context.ReportStepProgress(ReaderAccountStepProgressCodes.CheckingAccountExists);

        var result = await ValidateReaderAccountNotExistsSharedStep.CheckAsync(
            requestClient,
            new ValidateUserExists { Email = context.ConfirmRequest.Email },
            token);
        if (result is Some<bool> success && success.Value)
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
