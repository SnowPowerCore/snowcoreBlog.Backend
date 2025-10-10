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
using System.Collections.Concurrent;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class GetTokenForReaderAccountStep(IHttpContextAccessor httpContextAccessor,
                                          IScopedClientFactory clientFactory,
                                          IRequestClient<GetUserTokenPairWithPayload> requestClient,
                                          IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var currentUserId = context.GetFromData<Guid>(ReaderAccountUserConstants.CurrentUserId);

        var readerTokenReq = tokenReqOpts.Value.ToGetUserTokenPairWithPayload();
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountEmailClaimKey, context.LoginByAssertion.Email);
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountUserIdClaimKey, currentUserId.ToString());

        var configuredProviders = tokenReqOpts.Value.ClaimProviderServices ?? [];
        if (configuredProviders.Count > 0)
        {
            var requestId = Guid.NewGuid();
            var claimsTasks = new ConcurrentBag<Task<Response<ReaderClaimsResponse>>>();

            try
            {
                // send one request directly to each configured provider queue (provider string resolved to queue name)
                foreach (var provider in configuredProviders)
                {
                    var req = new RequestReaderClaims
                    {
                        RequestId = requestId,
                        UserId = currentUserId,
                        Email = context.LoginByAssertion.Email,
                        TargetServiceName = provider
                    };

                    var client = clientFactory.CreateRequestClient<RequestReaderClaims>(
                        new Uri($"queue:{provider}"), timeout: RequestTimeout.After(m: 1));

                    claimsTasks.Add(client.GetResponse<ReaderClaimsResponse>(req));
                }

                // wait until all providers responded or the operation was cancelled externally
                var allTasks = claimsTasks.ToArray();
                try
                {
                    // WaitAsync will throw OperationCanceledException if token is cancelled
                    await Task.WhenAll(allTasks).WaitAsync(token);
                }
                catch (OperationCanceledException) { }

                // merge collected claims (only from expected providers)
                foreach (var claimsProviderTask in claimsTasks)
                {
                    if (claimsProviderTask.IsCompletedSuccessfully)
                    {
                        var resp = claimsProviderTask.Result;
                        foreach (var kv in resp.Message.Claims)
                        {
                            readerTokenReq.Claims[kv.Key] = kv.Value;
                        }
                    }
                }
            }
            catch { }
        }

        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            readerTokenReq, timeout: RequestTimeout.After(m: 1));
        if (result.Message.IsSuccess)
        {
            var curPair = result.Message.Value;
            if (!string.IsNullOrWhiteSpace(curPair!.AccessToken))
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