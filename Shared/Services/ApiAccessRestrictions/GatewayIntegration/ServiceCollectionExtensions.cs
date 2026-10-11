using StackExchange.Redis;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAccessRestrictionsGatewayIntegration(this IServiceCollection services)
    {
        // Used by gateway middleware background service to refresh the restrictions snapshot.
        // Per-request checks are done in-process (no per-request HTTP call).
        // The host name is an Aspire service name resolved via service discovery,
        // so AddServiceDiscovery() is required (otherwise DNS lookup fails).
        services.AddHttpClient("ApiAccessRestrictions", static client =>
        {
            client.BaseAddress = new Uri("https://backend-apiaccessrestrictions");
        })
        .AddServiceDiscovery();

        services.AddSingleton<IApiAccessRestrictionsSnapshotStore, ApiAccessRestrictionsSnapshotStore>();
        services.AddSingleton<ILocalApiAccessRestrictionsEvaluator, LocalApiAccessRestrictionsEvaluator>();
        services.AddSingleton<IApiAccessRestrictionsSnapshotRefresher, ApiAccessRestrictionsSnapshotRefresher>();
        services.AddHostedService<ApiAccessRestrictionsSnapshotRefreshHostedService>();

        // Recall store requires an IConnectionMultiplexer (registered by AddRedisClient
        // in the host when a connection string is available); otherwise fall back to a
        // no-op store so post-response restriction decisions are simply not persisted.
        services.AddSingleton<IApiAccessRecallStore>(static serviceProvider =>
        {
            var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();
            return connectionMultiplexer is not default(IConnectionMultiplexer)
                ? new RedisApiAccessRecallStore(connectionMultiplexer)
                : NoOpApiAccessRecallStore.Instance;
        });

        return services;
    }
}