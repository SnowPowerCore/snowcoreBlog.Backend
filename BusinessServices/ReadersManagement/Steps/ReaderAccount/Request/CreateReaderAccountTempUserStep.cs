using System.Web;
using MassTransit;
using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Options;
using snowcoreBlog.Backend.Core.Resources;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;

public class CreateReaderAccountTempUserStep(IRequestClient<CreateTempUser> client,
                                             IOptions<ProjectOptions> projectOptions,
                                             IPublishEndpoint publishEndpoint) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IMaybe<RequestReaderAccountCreationResultDto>>
{
    public async Task<IMaybe<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<TempUserCreationResult>>(
            context.CreateRequest.ToCreateTempUser(), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;
            var verificationTokenExpDateStr = responseObj!.VerificationTokenExpirationDate.ToString("O");

            var builder = new UriBuilder(projectOptions.Value.PublicFrontendDomain + RouteResources.CompleteRegistrationRoute);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[nameof(responseObj.Email)] = responseObj.Email;
            query[nameof(responseObj.VerificationToken)] = responseObj.VerificationToken;
            query[nameof(responseObj.VerificationTokenExpirationDate)] = verificationTokenExpDateStr;
            builder.Query = query.ToString();
            var url = builder.ToString();

            await publishEndpoint.Publish<ReaderAccountTempUserCreated>(
                new(responseObj.FirstName,
                    responseObj.Email,
                    url,
                    verificationTokenExpDateStr), token);

            return Maybe.Create<RequestReaderAccountCreationResultDto>(new(responseObj.Id));
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountUserConstants.TempUserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}