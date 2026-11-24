using System.Text.Json;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;

/// <summary>
/// Default provider: reuses the stored auth-relevant headers as-is. Services with rotating
/// credentials (refresh tokens) replace this registration.
/// </summary>
public sealed class PassthroughReplayAuthenticationProvider(
    ILogger<PassthroughReplayAuthenticationProvider> logger) : IReplayAuthenticationProvider
{
    private static readonly JsonSerializerOptions SnapshotOptions = new(JsonSerializerDefaults.Web);

    public Task<IReadOnlyDictionary<string, string>?> ResolveReplayHeadersAsync(PersistedRequestEntity request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.HeadersJson))
        {
            return Task.FromResult<IReadOnlyDictionary<string, string>?>(new Dictionary<string, string>(StringComparer.Ordinal));
        }

        try
        {
            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(request.HeadersJson, SnapshotOptions);
            return Task.FromResult<IReadOnlyDictionary<string, string>?>(
                headers is default(Dictionary<string, string>)
                    ? new Dictionary<string, string>(StringComparer.Ordinal)
                    : new Dictionary<string, string>(headers, StringComparer.Ordinal));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Replay header snapshot for request {RequestId} could not be deserialized.", request.Id);
            return Task.FromResult<IReadOnlyDictionary<string, string>?>(null);
        }
    }
}
