using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Shared.Core.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class ResolveRefreshTokenStep(IHttpContextAccessor httpContextAccessor)
    : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderAccountTokenResultDto>>
{
    public async Task<IMaybe<RefreshReaderAccountTokenResultDto>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        // Prefer the backend-named cookie, then fall back to the BitzArt.Blazor.Auth
        // cookie (the only refresh cookie present when sign-in completed through
        // the Blazor host).
        var cookieToken = default(string);
        if (httpContext?.Request.Cookies.TryGetValue(AuthCookieConstants.UserRefreshTokenCookieName, out var backendCookieToken) == true &&
            !string.IsNullOrWhiteSpace(backendCookieToken))
        {
            cookieToken = backendCookieToken;
        }
        else if (httpContext?.Request.Cookies.TryGetValue(AuthCookieConstants.BitzArtRefreshTokenCookieName, out var bitzArtCookieToken) == true &&
                 !string.IsNullOrWhiteSpace(bitzArtCookieToken))
        {
            cookieToken = bitzArtCookieToken;
        }

        context.RefreshTokenFromCookie = cookieToken;

        var resolved = !string.IsNullOrWhiteSpace(context.RefreshTokenFromCookie)
            ? context.RefreshTokenFromCookie
            : context.RefreshTokenFromBody;

        if (string.IsNullOrWhiteSpace(resolved))
        {
            return RefreshTokenMissingError<RefreshReaderAccountTokenResultDto>.Create(
                "Missing refresh token.");
        }

        // If both are present, treat a mismatch as suspicious.
        if (!string.IsNullOrWhiteSpace(context.RefreshTokenFromCookie) &&
            !string.IsNullOrWhiteSpace(context.RefreshTokenFromBody) &&
            !string.Equals(context.RefreshTokenFromCookie, context.RefreshTokenFromBody, StringComparison.Ordinal))
        {
            return RefreshTokenMismatchError<RefreshReaderAccountTokenResultDto>.Create(
                "Refresh token cookie and body do not match.");
        }

        context.RefreshToken = resolved;

        return await next(context, token);
    }
}
