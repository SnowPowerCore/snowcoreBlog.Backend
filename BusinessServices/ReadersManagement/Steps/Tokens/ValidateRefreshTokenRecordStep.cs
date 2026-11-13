using System.Text.Json;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class ValidateRefreshTokenRecordStep(IConnectionMultiplexer redis)
    : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderAccountTokenResultDto>>
{
    public async Task<IMaybe<RefreshReaderAccountTokenResultDto>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var refreshToken = context.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return RefreshTokenMissingError<RefreshReaderAccountTokenResultDto>.Create(
                "Missing refresh token.");
        }

        var db = redis.GetDatabase();
        var recordKey = ReaderRefreshTokenConstants.RefreshTokenKey(refreshToken);
        var recordJson = await db.StringGetAsync(recordKey);
        if (!recordJson.HasValue)
        {
            return RefreshTokenInvalidError<RefreshReaderAccountTokenResultDto>.Create(
                "Invalid refresh token.");
        }

        ReaderRefreshTokenRecord? record;
        try
        {
            record = JsonSerializer.Deserialize((ReadOnlySpan<byte>)recordJson!, CoreSerializationContext.Default.ReaderRefreshTokenRecord);
        }
        catch
        {
            await db.KeyDeleteAsync(recordKey);
            return RefreshTokenInvalidError<RefreshReaderAccountTokenResultDto>.Create(
                "Invalid refresh token.");
        }

        if (record is default(ReaderRefreshTokenRecord) || record.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            await db.KeyDeleteAsync(recordKey);
            return RefreshTokenExpiredError<RefreshReaderAccountTokenResultDto>.Create(
                "Refresh token expired.");
        }

        var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(record.UserId);
        var currentForUser = await db.StringGetAsync(userCurrentKey);
        if (!currentForUser.HasValue || !string.Equals(currentForUser.ToString(), refreshToken, StringComparison.Ordinal))
        {
            return RefreshTokenRevokedError<RefreshReaderAccountTokenResultDto>.Create(
                "Refresh token revoked.");
        }

        context.Record = record;
        return await next(context, token);
    }
}
