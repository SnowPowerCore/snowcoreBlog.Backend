namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces;

public interface IApiAccessRestrictionsSnapshotRefresher
{
    /// <summary>
    /// Fetches the latest restrictions snapshot from the ApiAccessRestrictions service
    /// and swaps it into the snapshot store. Shared by the periodic polling hosted
    /// service and the instant-refresh MassTransit consumer in the gateway.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken);
}