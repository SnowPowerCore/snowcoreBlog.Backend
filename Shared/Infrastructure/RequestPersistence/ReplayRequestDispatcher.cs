using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Re-dispatches a stored request over HTTP loopback against the running service itself. The
/// replay is a completely ordinary request: same route, same payload, same idempotency key вЂ”
/// antiforgery is satisfied by minting a fresh token from the service's own token endpoint, and
/// authentication comes from <see cref="IReplayAuthenticationProvider"/>.
/// </summary>
public sealed class ReplayRequestDispatcher(
    IHttpClientFactory httpClientFactory,
    IServer server,
    IReplayAuthenticationProvider authProvider,
    IOptions<PersistRequestOptions> options,
    ILogger<ReplayRequestDispatcher> logger)
{
    public async Task<ReplayOutcome> DispatchAsync(PersistedRequestEntity request, CancellationToken ct)
    {
        var headers = await authProvider.ResolveReplayHeadersAsync(request, ct);
        if (headers is default(IReadOnlyDictionary<string, string>))
        {
            return ReplayOutcome.AuthFailed;
        }

        var baseUrl = server.Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault();
        if (string.IsNullOrEmpty(baseUrl))
        {
            return ReplayOutcome.NoSelfAddress;
        }

        var client = httpClientFactory.CreateClient(PersistRequestStorageExtensions.ReplayHttpClientName);
        var token = await TryGetAntiforgeryTokenAsync(client, baseUrl, headers, ct);
        if (token is null)
        {
            return ReplayOutcome.AntiforgeryUnavailable;
        }

        var uri = $"{baseUrl.TrimEnd('/')}{request.Path}{request.QueryString}";
        using var message = new HttpRequestMessage(new HttpMethod(request.Method), uri);
        foreach (var (name, value) in headers)
        {
            message.Headers.TryAddWithoutValidation(name, value);
        }

        message.Headers.Add(HeaderKeyConstants.IdempotencyKey, request.IdempotencyKey);
        message.Headers.Add(HeaderKeyConstants.ReplayMarker, bool.TrueString);
        message.Headers.Add(token.Value.HeaderName ?? "RequestVerificationToken", token.Value.RequestToken);

        if (!string.IsNullOrEmpty(request.RequestPayload))
        {
            message.Content = new StringContent(
                request.RequestPayload,
                Encoding.UTF8,
                string.IsNullOrEmpty(request.ContentType) ? "application/json" : request.ContentType);
        }

        using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, ct);
        var isStreaming = response.Content.Headers.ContentType?.MediaType?.Contains("event-stream") == true;
        var body = isStreaming ? null : await response.Content.ReadAsStringAsync(ct);

        var code = (int)response.StatusCode;
        var status = code switch
        {
            >= 200 and < 300 => RequestPersistenceStatus.Succeeded,
            >= 400 and < 500 => RequestPersistenceStatus.Failed,
            _ => RequestPersistenceStatus.RetryNeeded
        };
        return new ReplayOutcome(status, code, body, null);
    }

    private async Task<(string RequestToken, string HeaderName)?> TryGetAntiforgeryTokenAsync(
        HttpClient client,
        string baseUrl,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl.TrimEnd('/')}{options.Value.AntiforgeryTokenPath}");
            foreach (var (name, value) in headers)
            {
                request.Headers.TryAddWithoutValidation(name, value);
            }

            using var response = await client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Antiforgery token endpoint returned {StatusCode} during replay preparation.", (int)response.StatusCode);
                return null;
            }

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            var data = document.RootElement.GetProperty("data");
            var requestToken = data.GetProperty("requestToken").GetString();
            var headerName = data.TryGetProperty("headerName", out var headerNameElement) ? headerNameElement.GetString() : null;
            return string.IsNullOrEmpty(requestToken) ? null : (requestToken, headerName ?? "RequestVerificationToken");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Antiforgery token acquisition failed during replay preparation.");
            return null;
        }
    }
}
