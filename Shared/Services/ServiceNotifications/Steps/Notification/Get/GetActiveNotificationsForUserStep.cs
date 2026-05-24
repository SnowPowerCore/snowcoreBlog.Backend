using System.Security.Claims;
using MassTransit;
using MaybeResults;
using System.Text.Json;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;

public class GetActiveNotificationsForUserStep(IHttpContextAccessor httpContextAccessor,
                                               IScopedClientFactory clientFactory,
                                               IConnectionMultiplexer redis)
    : IStep<GetActiveNotificationsDelegate, GetActiveNotificationsContext, IMaybe<List<NotificationDto>>>
{
    private const string CacheKeyPrefix = "notifications:active:user";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

    public async Task<IMaybe<List<NotificationDto>>> InvokeAsync(
        GetActiveNotificationsContext context,
        GetActiveNotificationsDelegate next,
        CancellationToken token = default)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null || user.Identity is null || !user.Identity.IsAuthenticated)
            return await next(context, token);

        var userIdStr = user.FindFirst(ReaderAccountClaimConstants.ReaderAccountUserIdClaimKey)?.Value
                        ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return await next(context, token);

        // Read configured notification providers from Redis
        var db = redis.GetDatabase();
        var redisVal = await db.StringGetAsync(NotificationServiceProvidersConstants.RedisKey);
        List<string> configuredProviders = [];

        if (redisVal.HasValue && !string.IsNullOrWhiteSpace(redisVal))
        {
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
            return Maybe.Create(new List<NotificationDto>());

        // Check per-user cache first (includes optional MaxCount)
        var cacheKey = context.MaxCount.HasValue
            ? $"{CacheKeyPrefix}:{userId}:{context.MaxCount.Value}"
            : $"{CacheKeyPrefix}:{userId}:all";

        var cached = await db.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            try
            {
                var fromCache = JsonSerializer.Deserialize(cached.ToString(), CoreSerializationContext.Default.ListNotificationDto);
                if (fromCache is not default(List<NotificationDto>))
                    return Maybe.Create(fromCache);
            }
            catch { /* deserialization error - fall through to provider calls */ }
        }

        var requestId = Guid.NewGuid();
        var notificationsTasks = new Task<Response<UserNotificationsResponse>>[configuredProviders.Count];

        try
        {
            for (var i = 0; i < configuredProviders.Count; i++)
            {
                var provider = configuredProviders[i];

                var req = new RequestUserNotifications
                {
                    RequestId = requestId,
                    UserId = userId,
                    TargetServiceName = provider
                };

                var client = clientFactory.CreateRequestClient<RequestUserNotifications>(
                    new Uri($"queue:{provider}"), timeout: RequestTimeout.After(m: 1));

                notificationsTasks[i] = client.GetResponse<UserNotificationsResponse>(req);
            }

            try
            {
                await Task.WhenAll(notificationsTasks).WaitAsync(token);
            }
            catch (OperationCanceledException) { }

            var aggregated = new List<NotificationDto>();
            var seen = new HashSet<Guid>();

            foreach (var t in notificationsTasks)
            {
                if (t is not null && t.IsCompletedSuccessfully)
                {
                    var resp = t.Result;
                    foreach (var n in resp.Message.Notifications ?? new())
                    {
                        if (n is not null && seen.Add(n.Id))
                            aggregated.Add(n);
                    }
                }
            }

            // Cache the aggregated result for the user to avoid frequent provider fan-out
            try
            {
                var json = JsonSerializer.Serialize(aggregated, CoreSerializationContext.Default.ListNotificationDto);
                await db.StringSetAsync(cacheKey, json, CacheTtl);
            }
            catch { /* caching failure should not break the request */ }

            return Maybe.Create(aggregated.Count > 0 ? aggregated : []);
        }
        catch { }

        return Maybe.Create(new List<NotificationDto>());
    }
}