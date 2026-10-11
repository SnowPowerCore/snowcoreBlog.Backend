namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

/// <summary>
/// Periodic safety-net poll: keeps the gateway snapshot converging even when a
/// change event from the ApiAccessRestrictions service is missed. Instant
/// propagation is handled by the ApiAccessRestrictionsChanged consumer.
/// </summary>
public sealed class ApiAccessRestrictionsSnapshotRefreshHostedService(IApiAccessRestrictionsSnapshotRefresher refresher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        await refresher.RefreshAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await refresher.RefreshAsync(stoppingToken);
        }
    }
}