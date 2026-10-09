using System.Net;
using System.Text.Json;
using MassTransit;
using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.Backend.ReadersManagement.Models;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.ReadersManagement.Services;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class RotateReaderTokenPairStep(
    IHttpContextAccessor httpContextAccessor,
    IRequestClient<GetUserTokenPairWithPayload> requestClient,
    IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts,
    ReaderAccountTokenPairIssuer tokenPairIssuer,
    IConnectionMultiplexer redis) : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderJwtPairOperationResult>>
{
    public async Task<IMaybe<RefreshReaderJwtPairOperationResult>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var refreshToken = context.RefreshToken;
        var record = context.Record;
        if (string.IsNullOrWhiteSpace(refreshToken) || record is default(ReaderRefreshTokenRecord))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Invalid refresh token"], (int)HttpStatusCode.Unauthorized)
            });
        }

        var tokenRequest = tokenReqOpts.Value.ToGetUserTokenPairWithPayload();
        foreach (var kv in record.Claims)
            tokenRequest.Claims[kv.Key] = kv.Value;

        // Re-ask the configured claim providers so claims granted after sign-in
        // (e.g. a reader that became an author) are picked up without forcing a
        // full re-authentication. Base claims from the stored record stay in
        // place; fresh provider claims override them. Provider outages are
        // tolerated by the issuer — the record claims are then kept as-is.
        var freshClaims = await tokenPairIssuer.MergeExternalClaimsAsync(
            record.UserId,
            record.Claims.GetValueOrDefault(ReaderAccountClaimConstants.ReaderAccountEmailClaimKey) ?? string.Empty,
            token);
        foreach (var kv in freshClaims)
            tokenRequest.Claims[kv.Key] = kv.Value;

        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            tokenRequest, timeout: RequestTimeout.After(m: 1));

        if (!result.Message.IsSuccess || result.Message.Value is default(UserTokenPairWithPayloadGenerated) || string.IsNullOrWhiteSpace(result.Message.Value.AccessToken))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Unable to refresh token"], (int)HttpStatusCode.Unauthorized)
            });
        }

        var newPair = result.Message.Value;
        context.NewPair = newPair;

        var db = redis.GetDatabase();

        // Rotate: delete old token record and store the new one.
        await db.KeyDeleteAsync(ReaderRefreshTokenConstants.RefreshTokenKey(refreshToken));

        var newRecord = new ReaderRefreshTokenRecord
        {
            UserId = record.UserId,
            // Persist the merged claims (base + fresh provider claims) so the
            // next rotation keeps the refreshed claim set.
            Claims = new Dictionary<string, string>(tokenRequest.Claims, StringComparer.Ordinal),
            ExpiresAt = newPair.RefreshTokenExpiresAt
        };

        var newRecordKey = ReaderRefreshTokenConstants.RefreshTokenKey(newPair.RefreshToken);
        var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(record.UserId);

        var ttl = newPair.RefreshTokenExpiresAt - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero)
            ttl = TimeSpan.FromMinutes(tokenReqOpts.Value.RefreshTokenValidityDurationInMinutes);

        await db.StringSetAsync(newRecordKey, JsonSerializer.Serialize(newRecord, CoreSerializationContext.Default.ReaderRefreshTokenRecord), expiry: ttl);
        await db.StringSetAsync(userCurrentKey, newPair.RefreshToken, expiry: ttl);

        // Set cookies on response.
        var currentCookies = httpContextAccessor.HttpContext?.Response.Cookies;
        currentCookies?.Append(AuthCookieConstants.UserAccessTokenCookieName, newPair.AccessToken,
            new CookieOptions { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.AccessTokenValidityDurationInMinutes) });
        currentCookies?.Append(AuthCookieConstants.UserRefreshTokenCookieName, newPair.RefreshToken,
            new CookieOptions { MaxAge = TimeSpan.FromMinutes(tokenReqOpts.Value.RefreshTokenValidityDurationInMinutes) });

        var opResult = new RefreshReaderJwtPairOperationResult
        {
            HttpStatusCode = (int)HttpStatusCode.OK,
            Response = new ApiResponse(default, 0, (int)HttpStatusCode.OK, Array.Empty<string>())
        };

        context.Result = opResult;
        return Maybe.Create(opResult);
    }
}
