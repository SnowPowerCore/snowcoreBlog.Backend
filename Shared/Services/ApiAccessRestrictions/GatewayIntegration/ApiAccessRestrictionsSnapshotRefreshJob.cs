using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Interfaces;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

/// <summary>
/// TickerQ job: periodic safety-net poll that keeps the gateway restrictions snapshot
/// converging even when a change event from the ApiAccessRestrictions service is missed.
/// Replaces the former <c>ApiAccessRestrictionsSnapshotRefreshHostedService</c>
/// (PeriodicTimer, 30s); the gateway host seeds the same 30s cron ticker at startup.
/// Instant propagation stays with the ApiAccessRestrictionsChanged consumer.
/// </summary>
public sealed class ApiAccessRestrictionsSnapshotRefreshJob(IApiAccessRestrictionsSnapshotRefresher refresher) : ITickerFunction
{
    // Mirrors the former PeriodicTimer cadence (30 seconds, 6-field cron with seconds).
    public const string CronExpression = "*/30 * * * * *";

    public async Task ExecuteAsync(TickerFunctionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await refresher.RefreshAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Host shutdown — next occurrence refreshes again.
        }
        // Remaining failures surface as a Failed occurrence on the dashboard; the next
        // scheduled poll retries, matching the former fire-and-forget loop semantics.
    }
}