using System.Net;
using System.Net.Mime;
using System.Text.Json;
using FastEndpoints;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Tokens;

/// <summary>
/// Signs the current reader out: deletes the refresh-token record from Redis
/// (so the stored pair can no longer be rotated) and expires both auth cookies
/// on the response. Intentionally AllowAnonymous — sign-out must never fail
/// with 401, and the caller is identified by the refresh-token cookie alone.
/// The antiforgery check is enabled because this is a state-changing action.
/// </summary>
public class RevokeReaderAccountTokenEndpoint(IHttpContextAccessor httpContextAccessor,
                                             IConnectionMultiplexer redis) : EndpointWithoutRequest<ApiResponse?>
{
    public override void Configure()
    {
        Post("tokens/revoke");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.Tokens)
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Cookies.TryGetValue(AuthCookieConstants.UserRefreshTokenCookieName, out var refreshToken) == true &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            var db = redis.GetDatabase();
            var recordKey = ReaderRefreshTokenConstants.RefreshTokenKey(refreshToken);

            // Atomic read-and-delete: a concurrent refresh must not resurrect the record.
            var recordJson = await db.StringGetDeleteAsync(recordKey);
            if (recordJson.HasValue)
            {
                var record = JsonSerializer.Deserialize(recordJson.ToString(), CoreSerializationContext.Default.ReaderRefreshTokenRecord);
                if (record is not default(ReaderRefreshTokenRecord))
                {
                    // Only clear the "current token" pointer when it still points at
                    // the revoked token — another login may have rotated it already.
                    var userCurrentKey = ReaderRefreshTokenConstants.UserCurrentRefreshTokenKey(record.UserId);
                    var current = await db.StringGetAsync(userCurrentKey);
                    if (current.HasValue && string.Equals(current.ToString(), refreshToken, StringComparison.Ordinal))
                        await db.KeyDeleteAsync(userCurrentKey);
                }
            }
        }

        // Always expire both cookies, even when no refresh cookie was present —
        // the browser pair must never survive a sign-out attempt.
        var responseCookies = context?.Response.Cookies;
        responseCookies?.Delete(AuthCookieConstants.UserAccessTokenCookieName);
        responseCookies?.Delete(AuthCookieConstants.UserRefreshTokenCookieName);

        await Send.ResponseAsync(
            new ApiResponse(default, 0, (int)HttpStatusCode.OK, Array.Empty<string>()),
            (int)HttpStatusCode.OK, ct);
    }
}
