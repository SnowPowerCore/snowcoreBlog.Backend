using System.Text.Json;
using FastEndpoints;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Tokens;

/// <summary>
/// Tolerant binder for the refresh endpoint's plain <see cref="string" /> request.
/// <list type="bullet">
/// <item>Empty body → <see cref="string.Empty" /> so the pipeline resolves the refresh token
/// from the HTTP-only cookie (proactive rotation path sends an empty payload).</item>
/// <item>JSON-string body ("token") → unwrapped to the raw token.</item>
/// <item>Anything else → passed through as-is; business validation happens in the
/// step pipeline, never as a JSON serializer error.</item>
/// </list>
/// Without this binder an empty JSON body makes FastEndpoints' default string
/// deserialization throw "The input does not contain any JSON tokens" and the request
/// fails with 400 before the step pipeline can even look at the cookie.
/// </summary>
public sealed class RefreshTokenRequestBinder : IRequestBinder<string>
{
    public async ValueTask<string> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var request = ctx.HttpContext.Request;

        // Content-Length may be absent for streamed bodies, so read the payload itself.
        string body;
        using (var reader = new StreamReader(request.Body))
        {
            body = await reader.ReadToEndAsync(ct);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.ValueKind == JsonValueKind.String
                ? doc.RootElement.GetString() ?? string.Empty
                : string.Empty;
        }
        catch (JsonException)
        {
            // Non-JSON payload (e.g. a raw token sent as text/plain) — pass through.
            return body;
        }
    }
}
