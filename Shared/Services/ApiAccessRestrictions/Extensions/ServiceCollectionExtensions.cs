using Apizr;
using Microsoft.Extensions.Http.Resilience;
using snowcoreBlog.Backend.ApiAccessRestrictions.Api;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAccessRestrictionsGatewayIntegration(this IServiceCollection services)
    {
        // Used by gateway middleware background service to refresh the restrictions snapshot.
        // Per-request checks are done in-process (no per-request HTTP call).
        // The typed call itself is an Apizr-managed Refit contract (IApiAccessRestrictionsApi).
        // The host name is an Aspire service name resolved via service discovery,
        // so AddServiceDiscovery() is required (otherwise DNS lookup fails).
        services.AddApizr(static registry => registry.AddManagerFor<IApiAccessRestrictionsApi>(static opts => opts
                .WithBaseAddress("https://backend-apiaccessrestrictions")),
            static options => options
                .ConfigureHttpClientBuilder(static builder => builder
                    .AddServiceDiscovery()
                    .AddStandardResilienceHandler(static config =>
                    {
                        config.Retry = new HttpRetryStrategyOptions
                        {
                            UseJitter = true,
                            MaxRetryAttempts = 3,
                            Delay = TimeSpan.FromSeconds(0.5)
                        };
                    })));

        services.AddSingleton<IApiAccessRestrictionsSnapshotStore, ApiAccessRestrictionsSnapshotStore>();
        services.AddSingleton<ILocalApiAccessRestrictionsEvaluator, LocalApiAccessRestrictionsEvaluator>();
        services.AddSingleton<IApiAccessRestrictionsSnapshotRefresher, ApiAccessRestrictionsSnapshotRefresher>();
        // The periodic snapshot refresh is a TickerQ job (ApiAccessRestrictionsSnapshotRefreshJobs);
        // the host wires TickerQ itself (see AspireYarpGateway YarpResourceExtensions).

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