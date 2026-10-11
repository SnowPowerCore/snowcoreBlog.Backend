namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

/// <summary>
/// Fallback used when no Redis connection is configured in the host process:
/// recall lookups never match and writes are discarded (fail-open behavior).
/// </summary>
public sealed class NoOpApiAccessRecallStore : IApiAccessRecallStore
{
    public static readonly NoOpApiAccessRecallStore Instance = new();

    private NoOpApiAccessRecallStore() { }

    public Task<ApiAccessRecallEntry?> TryGetAsync(string clientId, CancellationToken cancellationToken) =>
        Task.FromResult<ApiAccessRecallEntry?>(null);

    public Task SetAsync(string clientId, ApiAccessRecallEntry entry, TimeSpan timeToLive, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
