using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using snowcoreBlog.Backend.YarpGateway.Core.Contracts;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.Backend.IAM.Core.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class GetTokenForReaderAccountStep(IRequestClient<GetUserTokenPairWithPayload> requestClient,
                                          IOptions<ReaderAccountTokenRequirements> tokenReqOpts,
                                          IHttpContextAccessor httpContextAccessor) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            tokenReqOpts.Value.ToGetUserTokenPairWithPayload(), token);
        if (result.Message.IsSuccess)
        {
            var curPair = result.Message.Value;
            if (!string.IsNullOrEmpty(curPair!.AccessToken))
            {
                var currentCookies = httpContextAccessor.HttpContext?.Response.Cookies;
                currentCookies?.Append(AuthConstants.TokenCookieName, curPair.AccessToken,
                    new() { Expires = curPair.AccessTokenExpiresAt });
                currentCookies?.Append("refreshToken", curPair.RefreshToken,
                    new() { Expires = curPair.RefreshTokenExpiresAt });
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