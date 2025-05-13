using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class GetTokenForReaderAccountStep(IRequestClient<GetUserTokenPairWithPayload> requestClient,
                                          IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts,
                                          IHttpContextAccessor httpContextAccessor) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var currentUserId = context.GetFromData<Guid>(ReaderAccountUserConstants.CurrentUserId);

        var readerTokenReq = tokenReqOpts.Value.ToGetUserTokenPairWithPayload();
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountEmailClaimKey, context.LoginByAssertion.Email);
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountUserIdClaimKey, currentUserId.ToString());
        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            readerTokenReq, token);
        if (result.Message.IsSuccess)
        {
            var curPair = result.Message.Value;
            if (!string.IsNullOrEmpty(curPair!.AccessToken))
            {
                var currentCookies = httpContextAccessor.HttpContext?.Response.Cookies;
                currentCookies?.Append(AuthCookieConstants.UserAccessTokenCookieName, curPair.AccessToken,
                    new() { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.AccessTokenValidityDurationInMinutes) });
                currentCookies?.Append(AuthCookieConstants.UserRefreshTokenCookieName, curPair.RefreshToken,
                    new() { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.RefreshTokenValidityDurationInMinutes) });
                return Maybe.Create<LoginByAssertionResultDto>(new());
            }
            else
            {
                return ReaderAccountNotExistError<LoginByAssertionResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountUnableToLogIn);
            }
        }
        else
        {
            return ReaderAccountNotExistError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}