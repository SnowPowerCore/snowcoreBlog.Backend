using System.Net;
using MaybeResults;
using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Models;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class UseRefreshTokenLockStep(IConnectionMultiplexer redis)
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
        var lockKey = ReaderRefreshTokenConstants.RefreshTokenLockKey(refreshToken);
        context.LockKey = lockKey;

        var lockAcquired = await db.StringSetAsync(lockKey, "1", expiry: TimeSpan.FromSeconds(5), when: When.NotExists);
        if (!lockAcquired)
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Refresh already in progress"], (int)HttpStatusCode.Unauthorized)
            });
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
