using MassTransit;
using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class GetTokenForReaderAccountStep(IHttpContextAccessor httpContextAccessor,
                                          IScopedClientFactory clientFactory,
                                          IRequestClient<GetUserTokenPairWithPayload> requestClient,
                                          IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts,
                                          IConnectionMultiplexer redis) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var currentUserId = context.GetFromData<Guid>(ReaderAccountUserConstants.CurrentUserId);

        var readerTokenReq = tokenReqOpts.Value.ToGetUserTokenPairWithPayload();
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountEmailClaimKey, context.LoginByAssertion.Email);
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountUserIdClaimKey, currentUserId.ToString());

        // Try to read the list of claim provider service names from Redis first.
        var db = redis.GetDatabase();
        var redisVal = await db.StringGetAsync(ClaimServiceProvidersConstants.RedisKey);
        List<string> configuredProviders = [];

        if (redisVal.HasValue && !string.IsNullOrWhiteSpace(redisVal))
        {
            // stored as comma-separated values; keep only unique providers (case-insensitive)
            configuredProviders = redisVal.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        if (configuredProviders.Count > 0)
        {
            var requestId = Guid.NewGuid();
            var claimsTasks = new Task<Response<ReaderClaimsResponse>>[configuredProviders.Count];

            try
            {
                // send one request directly to each configured provider queue (provider string resolved to queue name)
                for (var i = 0; i < configuredProviders.Count; i++)
                {
                    var provider = configuredProviders[i];

                    var req = new RequestReaderClaims
                    {
                        RequestId = requestId,
                        UserId = currentUserId,
                        Email = context.LoginByAssertion.Email,
                        TargetServiceName = provider
                    };

                    var client = clientFactory.CreateRequestClient<RequestReaderClaims>(
                        new Uri($"queue:{provider}"), timeout: RequestTimeout.After(m: 1));

                    claimsTasks[i] = client.GetResponse<ReaderClaimsResponse>(req);
                }

                try
                {
                    // Wait until all providers responded or the operation was cancelled externally
                    // WaitAsync will throw OperationCanceledException if token is cancelled
                    await Task.WhenAll(claimsTasks).WaitAsync(token);
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

                // Persist the refresh token server-side for rotation/validation.
                var refreshRecord = new ReaderRefreshTokenRecord
                {
                    UserId = currentUserId,
                    Claims = readerTokenReq.Claims.ToDictionary(),
                    ExpiresAt = curPair.RefreshTokenExpiresAt
                };

                var refreshKey = ReaderRefreshTokenConstants.RefreshTokenKey(curPair.RefreshToken);
                var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(currentUserId);
                var ttl = curPair.RefreshTokenExpiresAt - DateTimeOffset.UtcNow;
                if (ttl <= TimeSpan.Zero)
                    ttl = TimeSpan.FromMinutes(tokenReqOpts.Value.RefreshTokenValidityDurationInMinutes);

                // Revoke any previously issued refresh token for this user.
                var previousToken = await db.StringGetAsync(userCurrentKey);
                if (previousToken.HasValue && !string.IsNullOrWhiteSpace(previousToken.ToString()))
                {
                    await db.KeyDeleteAsync(ReaderRefreshTokenConstants.RefreshTokenKey(previousToken.ToString()));
                }

                await db.StringSetAsync(refreshKey, System.Text.Json.JsonSerializer.Serialize(refreshRecord, CoreSerializationContext.Default.ReaderRefreshTokenRecord), expiry: ttl);
                await db.StringSetAsync(userCurrentKey, curPair.RefreshToken, expiry: ttl);
                return Maybe.Create<LoginByAssertionResultDto>(new());
            }
            else
            {
                return ReaderAccountNotExistsError<LoginByAssertionResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountUnableToLogIn);
            }
        }
        else
        {
            return ReaderAccountNotExistsError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}