using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateReaderAccountTempUserStep(IRequestClient<CreateTempUser> client,
                                             IPublishEndpoint publishEndpoint) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IMaybe<RequestReaderAccountCreationResultDto>>
{
    public async Task<IMaybe<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<TempUserCreationResult>>(
            context.CreateRequest.ToCreateTempUser(), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;

            await publishEndpoint.Publish<ReaderAccountTempUserCreated>(
                new(responseObj!.FirstName,
                    responseObj.Email,
                    responseObj.VerificationToken + responseObj.Email,
                    responseObj.VerificationTokenExpirationDate.ToString()), token);

            return Maybe.Create<RequestReaderAccountCreationResultDto>(new(responseObj.Id));
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountUserConstants.TempUserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}