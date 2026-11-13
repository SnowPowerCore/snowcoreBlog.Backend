using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class UseRefreshTokenLockStep(IConnectionMultiplexer redis)
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
        var lockKey = ReaderRefreshTokenConstants.RefreshTokenLockKey(refreshToken);
        context.LockKey = lockKey;

        var lockAcquired = await db.StringSetAsync(lockKey, "1", expiry: TimeSpan.FromSeconds(5), when: When.NotExists);
        if (!lockAcquired)
        {
            return RefreshAlreadyInProgressError<RefreshReaderAccountTokenResultDto>.Create(
                "A refresh for this token is already in progress.");
        }

        try
        {
            return await next(context, token);
        }
        finally
        {
            await db.KeyDeleteAsync(lockKey);
        }
    }
}
