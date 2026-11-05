using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TickerQ.Utilities;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// TickerQ support for the durable request persistence machinery: converts the configured
/// <see cref="PersistRequestOptions.RecoveryPollInterval"/> into a TickerQ 6-field cron
/// expression and seeds (migrates) the recurring recovery job into the persistence provider
/// used by the host (Redis when configured, otherwise the TickerQ in-memory provider).
/// <para>
/// <c>MigrateDefinedCronTickers</c> upserts by function name — created once, updated when the
/// configured interval changes, never duplicated — so calling it on every startup is safe and
/// keeps <c>PersistRequestOptions.RecoveryPollInterval</c> the single source of truth.
/// </para>
/// </summary>
public static class PersistRequestTickerQ
{
    /// <summary>
    /// Seeds or updates the recurring recovery cron ticker from
    /// <see cref="PersistRequestOptions.RecoveryPollInterval"/>. Call once during startup,
    /// after the host is built and before the TickerQ scheduler starts.
    /// </summary>
    public static async Task SeedRecoveryCronAsync(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<PersistRequestOptions>>().Value;
        var expression = CronFromInterval(options.RecoveryPollInterval);

        var provider = services.GetRequiredService<ITickerPersistenceProvider<TimeTickerEntity, CronTickerEntity>>();
        var functionName = TickerFunctionProvider.GetFunctionName<PersistRequestRecoveryJob>();
        await provider.MigrateDefinedCronTickers([(functionName, expression)]);
    }

    /// <summary>
    /// Converts a poll interval to a TickerQ cron expression (6 fields: second minute hour
    /// day month day-of-week). Sub-minute intervals become stepped seconds, whole minutes
    /// stepped minutes, whole hours stepped hours; anything else falls back to the closest
    /// supported whole-minute step.
    /// </summary>
    public static string CronFromInterval(TimeSpan interval)
    {
        var seconds = (int)Math.Ceiling(interval.TotalSeconds);
        if (seconds < 1)
        {
            seconds = 1;
        }

        return seconds switch
        {
            <= 59 => $"*/{seconds} * * * * *",
            _ when seconds % 3600 == 0 && seconds / 3600 <= 23 => $"0 0 */{seconds / 3600} * * *",
            _ when seconds % 60 == 0 && seconds / 60 <= 59 => $"0 */{seconds / 60} * * * *",
            _ => "0 * * * * *"
        };
    }
}
