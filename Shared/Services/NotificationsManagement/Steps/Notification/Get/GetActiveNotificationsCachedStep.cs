using System.Text.Json;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.Backend.NotificationsManagement.Delegates;
using snowcoreBlog.Backend.NotificationsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.NotificationsManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.NotificationsManagement.Steps.Notification.Get;

public class GetActiveNotificationsCachedStep(INotificationRepository notificationRepository, IConnectionMultiplexer redis) 
    : IStep<GetActiveNotificationsDelegate, GetActiveNotificationsContext, IMaybe<List<NotificationDto>>>
{
    private const string CacheKeyPrefix = "notifications:active";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5); // Short TTL for notifications

    public async Task<IMaybe<List<NotificationDto>>> InvokeAsync(
        GetActiveNotificationsContext context, 
        GetActiveNotificationsDelegate next, 
        CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var cacheKey = context.MaxCount.HasValue 
            ? $"{CacheKeyPrefix}:{context.MaxCount.Value}" 
            : $"{CacheKeyPrefix}:all";

        // Try to get from cache first
        var cached = await db.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            try
            {
                var fromCache = JsonSerializer.Deserialize(cached.ToString(), CoreSerializationContext.Default.ListNotificationDto);
                if (fromCache is not null)
                    return Maybe.Create(fromCache);
            }
            catch { /* Cache miss or deserialization error, fetch from DB */ }
        }

        // Fetch from database
        var notifications = await notificationRepository.GetActiveNotificationsAsync(context.MaxCount, token);
        var dtos = notifications.ToDtos().ToList();

        // Cache the result
        try
        {
            var json = JsonSerializer.Serialize(dtos, CoreSerializationContext.Default.ListNotificationDto);
            await db.StringSetAsync(cacheKey, json, CacheTtl);
        }
        catch { /* Caching failure should not break the request */ }

        return Maybe.Create(dtos);
    }
}
