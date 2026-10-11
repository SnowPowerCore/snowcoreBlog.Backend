using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public interface IApiAccessRestrictionsSnapshotRefresher
{
    /// <summary>
    /// Fetches the latest restrictions snapshot from the ApiAccessRestrictions service
    /// and swaps it into the snapshot store. Shared by the periodic polling hosted
    /// service and the instant-refresh MassTransit consumer in the gateway.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken);
}

public sealed class ApiAccessRestrictionsSnapshotRefresher(IHttpClientFactory httpClientFactory,
                                                           IApiAccessRestrictionsSnapshotStore store,
                                                           ILogger<ApiAccessRestrictionsSnapshotRefresher> logger) : IApiAccessRestrictionsSnapshotRefresher
{
    public async Task RefreshAsync(CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiAccessRestrictions");

            var snapshot = await client.GetFromJsonAsync(
                "snapshot/v1",
                CoreSerializationContext.Default.ApiAccessRestrictionsSnapshotDto,
                ct);
            if (snapshot is default(ApiAccessRestrictionsSnapshotDto))
                return;

            store.Update(snapshot);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "ApiAccessRestrictions snapshot refresh failed; keeping previous snapshot.");
        }
    }
}
