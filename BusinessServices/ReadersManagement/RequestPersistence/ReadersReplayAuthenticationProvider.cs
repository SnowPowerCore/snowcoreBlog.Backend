using System.Net;
using System.Text;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;

namespace snowcoreBlog.Backend.ReadersManagement.RequestPersistence;

/// <summary>
/// Re-authenticates replays via the service's own refresh-token rotation flow: the stored cookie
/// snapshot is attached to a cookie-only proactive rotation call; the rotated pair from the
/// response replaces the stored cookies. A definitive refresh rejection (401/403 — revoked or
/// expired refresh token) aborts the replay into terminal Failed (manual handling).
/// </summary>
public sealed class ReadersReplayAuthenticationProvider(
    IHttpClientFactory httpClientFactory,
    ILogger<ReadersReplayAuthenticationProvider> logger) : IReplayAuthenticationProvider
{
    private const string RefreshPath = "/v1/tokens/refresh";

    public async Task<IReadOnlyDictionary<string, string>?> ResolveReplayHeadersAsync(PersistedRequestEntity request, CancellationToken ct)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(request.HeadersJson))
        {
            try
            {
                var snapshot = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(request.HeadersJson, SnapshotSource.Options);
                if (snapshot is not default(Dictionary<string, string>))
                {
                    foreach (var (name, value) in snapshot)
                    {
                        headers[name] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Replay header snapshot for request {RequestId} could not be deserialized.", request.Id);
                return null;
            }
        }

        if (!headers.TryGetValue("Cookie", out var cookieHeader) || string.IsNullOrWhiteSpace(cookieHeader))
        {
            return headers;
        }

        try
        {
            var client = httpClientFactory.CreateClient(PersistRequestStorageExtensions.ReplayHttpClientName);
            using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, RefreshPath)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };
            refreshRequest.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

            using var response = await client.SendAsync(refreshRequest, ct);
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                // Refresh token revoked/expired — the original caller's session cannot be restored.
                logger.LogWarning("Replay re-auth for request {RequestId} rejected by the refresh flow.", request.Id);
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var rotated = MergeRotatedCookies(cookieHeader, response.Headers.GetValues("Set-Cookie"));
                headers["Cookie"] = rotated;
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Refresh endpoint unreachable — try replays with the stored credentials untouched.
            logger.LogWarning(ex, "Replay re-auth for request {RequestId} could not reach the refresh flow; using stored credentials.", request.Id);
        }

        return headers;
    }

    private static string MergeRotatedCookies(string originalCookieHeader, IEnumerable<string> setCookieValues)
    {
        var cookies = new List<string>();
        foreach (var raw in originalCookieHeader.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            cookies.Add(raw);
        }

        var replaced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var setCookie in setCookieValues)
        {
            var pair = setCookie.Split(';', 2)[0];
            var name = pair.Split('=', 2)[0].Trim();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            cookies.RemoveAll(existing => existing.Split('=', 2)[0].Trim().Equals(name, StringComparison.OrdinalIgnoreCase));
            cookies.Add(pair);
            replaced.Add(name);
        }

        return string.Join("; ", cookies);
    }
}

internal static class SnapshotSource
{
    public static readonly System.Text.Json.JsonSerializerOptions Options = new(System.Text.Json.JsonSerializerDefaults.Web);
}
