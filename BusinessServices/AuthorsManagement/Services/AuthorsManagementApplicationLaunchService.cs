using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Features;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.PublicApi.Extensions;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.AuthorsManagement.Services;

public class AuthorsManagementApplicationLaunchService : IApplicationLaunchService
{
    private readonly IConnectionMultiplexer _redis;

    public AuthorsManagementApplicationLaunchService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task InitAsync()
    {
        try
        {
            var authorProvider = StringExtensions.TrimEnd(nameof(ReturnClaimsIfUserAuthorConsumer), "Consumer");
            var db = _redis.GetDatabase();
            var redisVal = await db.StringGetAsync(ClaimServiceProvidersConstants.RedisKey);
            if (redisVal.HasValue && !string.IsNullOrWhiteSpace(redisVal))
            {
                var currentProviders = redisVal.ToString();
                currentProviders += "," + authorProvider;
                await db.StringSetAsync(ClaimServiceProvidersConstants.RedisKey, currentProviders);
            }
            else
            {
                await db.StringSetAsync(ClaimServiceProvidersConstants.RedisKey, authorProvider);
            }
        }
        catch { }
    }
}