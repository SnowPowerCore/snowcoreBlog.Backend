using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Infrastructure.StepProgress;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;

public class CreateReaderAccountUserWithAuthenticatorStep(IRequestClient<CreateUserWithAuthenticator> client,
                                                         IPublishEndpoint publishEndpoint) : IStep<ConfirmCreateReaderAccountWithAuthenticatorDelegate, ConfirmCreateReaderAccountWithAuthenticatorContext, IMaybe<ReaderAccountCreatedDto>>
{
    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountWithAuthenticatorContext context, ConfirmCreateReaderAccountWithAuthenticatorDelegate next, CancellationToken token = default)
    {
        context.ReportStepProgress(ReaderAccountStepProgressCodes.CreatingUserAccount);

        var response = await client.GetResponse<DataResult<UserCreationResult>>(
            new CreateUserWithAuthenticator
            {
                Email = context.ConfirmRequest.Email,
                TempUserVerificationToken = context.ConfirmRequest.VerificationToken,
                Code = context.ConfirmRequest.Code
            }, token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response.Message;
            var userCreationResult = responseObj.Value;
            var readerAccountUserCreated = new ReaderAccountUserCreated(userCreationResult!.Id, userCreationResult.Email);

            context.SetDataWith(
                ReaderAccountUserConstants.CreateUserForReaderAccountResult,
                responseObj);
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountUserResult,
                Maybe.Create(readerAccountUserCreated));

            await publishEndpoint.Publish(readerAccountUserCreated, token);

            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountUserConstants.UserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}
