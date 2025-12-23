using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public sealed class ApiAccessRestrictionsSnapshotRefreshHostedService(IHttpClientFactory httpClientFactory,
                                                                      IApiAccessRestrictionsSnapshotStore store,
                                                                      ILogger<ApiAccessRestrictionsSnapshotRefreshHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        await RefreshOnce(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RefreshOnce(stoppingToken);
        }
    }

    private async Task RefreshOnce(CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiAccessRestrictions");

            var json = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            }.SetJsonSerializationContext();

            var snapshot = await client.GetFromJsonAsync(
                "/access/snapshot/v1",
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