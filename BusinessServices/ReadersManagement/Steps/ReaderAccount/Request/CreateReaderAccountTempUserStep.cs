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
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Options;
using System.Web;
using snowcoreBlog.Frontend.Core.Resources;

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
            var verificationTokenExpDateStr = responseObj!.VerificationTokenExpirationDate.ToString();

            var builder = new UriBuilder(projectOptions.Value.PublicFrontendDomain + RouteResources.CompleteRegistrationRoute);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[nameof(responseObj.Email)] = HttpUtility.UrlEncode(responseObj.Email);
            query[nameof(responseObj.VerificationToken)] = HttpUtility.UrlEncode(responseObj.VerificationToken);
            query[nameof(responseObj.VerificationTokenExpirationDate)] = HttpUtility.UrlEncode(verificationTokenExpDateStr);
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