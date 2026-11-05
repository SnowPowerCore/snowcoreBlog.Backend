using snowcoreBlog.Backend.Infrastructure.RequestPersistence;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.Utilities;
using TickerQ.Utilities.Entities;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class TickerQConfigurationExtensions
{
    /// <summary>
    /// Registers TickerQ as the scheduler for the durable request persistence recovery poll.
    /// Uses TickerQ's in-memory persistence deliberately: the TickerQ 10.x Redis provider
    /// namespaces all keys under a fixed "tq:" prefix with no per-application partition, so
    /// pointing both this service and the gateway at the shared <c>cache</c> instance would
    /// make them claim each other's cron occurrences (function-not-found churn). Cron tickers
    /// are re-seeded on every startup regardless, so in-memory persistence loses nothing.
    /// The dashboard is enabled by default (basic-auth when credentials are configured under
    /// <c>TickerQ:Dashboard</c>).
    /// </summary>
    public static IServiceCollection AddTickerQConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.MapTicker<PersistRequestRecoveryJob>();
        services.AddTickerQ(options =>
        {
            ConfigureDashboard(options, configuration, requireAuthentication: false);
        });

        return services;
    }

    /// <summary>
    /// Performs the startup work previously done by the recovery worker's hosted-service
    /// constructor and <c>ExecuteAsync</c>: eagerly publishes the static
    /// <see cref="StepCheckpointCoordinator"/> reference (the woven step advices read it),
    /// seeds/migrates the recovery cron ticker from
    /// <see cref="PersistRequestOptions.RecoveryPollInterval"/>, and activates the TickerQ
    /// scheduler. Call once after <c>Build()</c> and before the application runs.
    /// </summary>
    public static async Task UseTickerQConfigurationAsync(this WebApplication app)
    {
        _ = app.Services.GetRequiredService<StepCheckpointCoordinator>();

        await PersistRequestTickerQ.SeedRecoveryCronAsync(app.Services);

        app.UseTickerQ();
    }

    private static void ConfigureDashboard(
        TickerOptionsBuilder<TimeTickerEntity, CronTickerEntity> options,
        IConfiguration configuration,
        bool requireAuthentication)
    {
        var enabled = configuration.GetValue("TickerQ:Dashboard:Enabled", !requireAuthentication);
        if (!enabled)
        {
            return;
        }

        var username = configuration["TickerQ:Dashboard:Username"];
        var password = configuration["TickerQ:Dashboard:Password"];
        var hasCredentials = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

        // Refuse to expose an unauthenticated dashboard on hosts that require auth
        // (the gateway sits on the public ingress).
        if (requireAuthentication && !hasCredentials)
        {
            return;
        }

        options.AddDashboard(dashboard =>
        {
            if (hasCredentials)
            {
                dashboard.WithBasicAuth(username!, password!);
            }
        });
    }
}
