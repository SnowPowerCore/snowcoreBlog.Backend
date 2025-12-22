using System.Net;
using System.Text.Json;
using MassTransit;
using MaybeResults;
using Microsoft.AspNetCore.Http;
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
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class RotateReaderTokenPairStep(
    IHttpContextAccessor httpContextAccessor,
    IRequestClient<GetUserTokenPairWithPayload> requestClient,
    IOptions<ReaderAccountTokenRequirementOptions> tokenReqOpts,
    IConnectionMultiplexer redis) : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderJwtPairOperationResult>>
{
    public async Task<IMaybe<RefreshReaderJwtPairOperationResult>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var refreshToken = context.RefreshToken;
        var record = context.Record;
        if (string.IsNullOrWhiteSpace(refreshToken) || record is null)
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

        var result = await requestClient.GetResponse<DataResult<UserTokenPairWithPayloadGenerated>>(
            tokenRequest, timeout: RequestTimeout.After(m: 1));

        if (!result.Message.IsSuccess || result.Message.Value is null || string.IsNullOrWhiteSpace(result.Message.Value.AccessToken))
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
            Claims = new Dictionary<string, string>(record.Claims, StringComparer.Ordinal),
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
