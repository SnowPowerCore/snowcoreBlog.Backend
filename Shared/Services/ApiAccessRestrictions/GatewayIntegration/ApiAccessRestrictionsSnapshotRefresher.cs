using Apizr;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public sealed class ApiAccessRestrictionsSnapshotRefresher(IApizrManager<Api.IApiAccessRestrictionsApi> api,
                                                           IApiAccessRestrictionsSnapshotStore store,
                                                           ILogger<ApiAccessRestrictionsSnapshotRefresher> logger) : IApiAccessRestrictionsSnapshotRefresher
{
    public async Task RefreshAsync(CancellationToken ct)
    {
        try
        {
            var snapshot = await api.ExecuteAsync(static (opt, client) => client.GetSnapshot(opt),
                o => o.WithCancellation(ct));
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