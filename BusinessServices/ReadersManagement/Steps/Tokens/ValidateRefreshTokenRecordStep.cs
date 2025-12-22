using System.Net;
using System.Text.Json;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Models;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class ValidateRefreshTokenRecordStep(IConnectionMultiplexer redis)
    : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderJwtPairOperationResult>>
{
    public async Task<IMaybe<RefreshReaderJwtPairOperationResult>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var refreshToken = context.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Missing refresh token"], (int)HttpStatusCode.BadRequest)
            });
        }

        var db = redis.GetDatabase();
        var recordKey = ReaderRefreshTokenConstants.RefreshTokenKey(refreshToken);
        var recordJson = await db.StringGetAsync(recordKey);
        if (!recordJson.HasValue)
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Invalid refresh token"], (int)HttpStatusCode.Unauthorized)
            });
        }

        ReaderRefreshTokenRecord? record;
        try
        {
            record = JsonSerializer.Deserialize(recordJson!, CoreSerializationContext.Default.ReaderRefreshTokenRecord);
        }
        catch
        {
            await db.KeyDeleteAsync(recordKey);
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Invalid refresh token"], (int)HttpStatusCode.Unauthorized)
            });
        }

        if (record is null || record.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            await db.KeyDeleteAsync(recordKey);
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Refresh token expired"], (int)HttpStatusCode.Unauthorized)
            });
        }

        var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(record.UserId);
        var currentForUser = await db.StringGetAsync(userCurrentKey);
        if (!currentForUser.HasValue || !string.Equals(currentForUser.ToString(), refreshToken, StringComparison.Ordinal))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Refresh token revoked"], (int)HttpStatusCode.Unauthorized)
            });
        }

        context.Record = record;
        return await next(context, token);
    }
}
