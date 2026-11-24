using System.Text.Json;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;

/// <summary>
/// Produces the headers a replayed request must carry. Implementations may re-authenticate using
/// the stored credential snapshot (e.g. refresh-token flow); returning <c>null</c> aborts the
/// replay into a terminal Failed state (manual handling).
/// </summary>
public interface IReplayAuthenticationProvider
{
    Task<IReadOnlyDictionary<string, string>?> ResolveReplayHeadersAsync(PersistedRequestEntity request, CancellationToken ct);
}
