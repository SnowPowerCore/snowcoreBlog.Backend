namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAccessRestrictionsGatewayIntegration(this IServiceCollection services)
    {
        // Used by gateway middleware background service to refresh the restrictions snapshot.
        // Per-request checks are done in-process (no per-request HTTP call).
        services.AddHttpClient("ApiAccessRestrictions", static client =>
        {
            client.BaseAddress = new Uri("https://backend-apiaccessrestrictions");
        });

        services.AddSingleton<IApiAccessRestrictionsSnapshotStore, ApiAccessRestrictionsSnapshotStore>();
        services.AddSingleton<ILocalApiAccessRestrictionsEvaluator, LocalApiAccessRestrictionsEvaluator>();
        services.AddHostedService<ApiAccessRestrictionsSnapshotRefreshHostedService>();
        return services;
    }
}