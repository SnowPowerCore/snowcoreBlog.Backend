using MassTransit;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.AspireYarpGateway.Features;

/// <summary>
/// Instant snapshot propagation: refreshes the gateway's in-memory restrictions
/// snapshot as soon as the ApiAccessRestrictions service reports a rule change.
/// Burst coalescing: a refresh already in flight swallows concurrent events; the
/// 30s periodic poll remains the eventual-consistency safety net.
/// </summary>
public sealed class ApiAccessRestrictionsChangedConsumer(
    IApiAccessRestrictionsSnapshotRefresher snapshotRefresher,
    ILogger<ApiAccessRestrictionsChangedConsumer> logger) : IConsumer<ApiAccessRestrictionsChanged>
{
    private static int _refreshing;

    public async Task Consume(ConsumeContext<ApiAccessRestrictionsChanged> context)
    {
        if (Interlocked.CompareExchange(ref _refreshing, 1, 0) != 0)
            return;

        try
        {
            await snapshotRefresher.RefreshAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ApiAccessRestrictions snapshot refresh triggered by a change event failed.");
        }
        finally
        {
            Interlocked.Exchange(ref _refreshing, 0);
        }
    }
}
