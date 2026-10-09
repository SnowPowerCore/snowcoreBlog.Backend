using MassTransit;
using MaybeResults;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Services;

/// <summary>
/// Issues and persists the JWT pair for a signed-in reader account, shared by
/// every login path (passkey assertion, one-time code, external providers):
/// mints the token pair through the gateway consumer, merges external claim
/// provider claims, appends both auth cookies to the response and persists the
/// rotating refresh-token record in Redis.
/// </summary>
public sealed class ReaderAccountTokenPairIssuer(IHttpContextAccessor httpContextAccessor,
                                                 IScopedClientFactory clientFactory,
                                                 IRequestClient<GetUserTokenPairWithPayload> requestClient,
                                                 IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts,
                                                 IConnectionMultiplexer redis)
{
    public async Task<(bool Success, IReadOnlyList<NoneDetail> Errors)> TryIssueAsync(
        Guid userId, string email, CancellationToken token = default)
    {
        var readerTokenReq = tokenReqOpts.Value.ToGetUserTokenPairWithPayload();
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountEmailClaimKey, email);
        readerTokenReq.Claims.Add(ReaderAccountClaimConstants.ReaderAccountUserIdClaimKey, userId.ToString());

        var mergedClaims = await MergeExternalClaimsAsync(userId, email, token);
        foreach (var claim in mergedClaims)
        {
            readerTokenReq.Claims[claim.Key] = claim.Value;
        }

        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            readerTokenReq, timeout: RequestTimeout.After(m: 1));
        if (!result.Message.IsSuccess || string.IsNullOrWhiteSpace(result.Message.Value!.AccessToken))
        {
            return (false, result.Message.Errors ?? []);
        }

        var curPair = result.Message.Value;
        var currentCookies = httpContextAccessor.HttpContext?.Response.Cookies;
        currentCookies?.Append(AuthCookieConstants.UserAccessTokenCookieName, curPair.AccessToken,
            new() { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.AccessTokenValidityDurationInMinutes) });
        currentCookies?.Append(AuthCookieConstants.UserRefreshTokenCookieName, curPair.RefreshToken,
            new() { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.RefreshTokenValidityDurationInMinutes) });

        // Persist the refresh token server-side for rotation/validation.
        var refreshRecord = new ReaderRefreshTokenRecord
        {
            UserId = userId,
            Claims = readerTokenReq.Claims.ToDictionary(),
            ExpiresAt = curPair.RefreshTokenExpiresAt
        };

        var db = redis.GetDatabase();
        var refreshKey = ReaderRefreshTokenConstants.RefreshTokenKey(curPair.RefreshToken);
        var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(userId);
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

        return (true, []);
    }

    /// <summary>
    /// Reads the list of configured claim provider service names from Redis and
    /// asks each one (via its dedicated request queue) for additional claims to
    /// merge into the token payload. Provider outages or timeouts are tolerated.
    /// Public so the token-refresh pipeline can re-query providers and pick up
    /// claims that were granted after sign-in (e.g. a reader becoming an author).
    /// </summary>
    public async Task<Dictionary<string, string>> MergeExternalClaimsAsync(Guid userId, string email, CancellationToken token)
    {
        var merged = new Dictionary<string, string>();

        var db = redis.GetDatabase();
        var redisVal = await db.StringGetAsync(ClaimServiceProvidersConstants.RedisKey);
        List<string> configuredProviders = [];

        if (redisVal.HasValue && !string.IsNullOrWhiteSpace(redisVal))
        {
            // stored as comma-separated values; keep only unique providers (case-insensitive)
            var distinctProviders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var orderedProviders = new List<string>();

            ReadOnlySpan<char> span = redisVal.ToString().AsSpan();
            while (!span.IsEmpty)
            {
                var commaIndex = span.IndexOf(',');
                ReadOnlySpan<char> segment;
                if (commaIndex < 0)
                {
                    segment = span;
                    span = default;
                }
                else
                {
                    segment = span.Slice(0, commaIndex);
                    span = span.Slice(commaIndex + 1);
                }

                segment = segment.Trim();
                if (segment.Length == 0)
                    continue;

                var provider = segment.ToString();
                if (distinctProviders.Add(provider))
                    orderedProviders.Add(provider);
            }

            configuredProviders = orderedProviders;
        }

        if (configuredProviders.Count == 0)
            return merged;

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
                    UserId = userId,
                    Email = email,
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
                        merged[kv.Key] = kv.Value;
                    }
                }
            }
        }
        catch { }

        return merged;
    }
}
