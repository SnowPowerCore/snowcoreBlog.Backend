using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Models;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;

public sealed class ResolveRefreshTokenStep(IHttpContextAccessor httpContextAccessor)
    : IStep<RefreshReaderJwtPairDelegate, RefreshReaderJwtPairContext, IMaybe<RefreshReaderJwtPairOperationResult>>
{
    public async Task<IMaybe<RefreshReaderJwtPairOperationResult>> InvokeAsync(
        RefreshReaderJwtPairContext context,
        RefreshReaderJwtPairDelegate next,
        CancellationToken token = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        context.RefreshTokenFromCookie = httpContext?.Request.Cookies.TryGetValue(AuthCookieConstants.UserRefreshTokenCookieName, out var cookieToken) == true
            ? cookieToken
            : null;

        var resolved = !string.IsNullOrWhiteSpace(context.RefreshTokenFromCookie)
            ? context.RefreshTokenFromCookie
            : context.RefreshTokenFromBody;

        if (string.IsNullOrWhiteSpace(resolved))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Missing refresh token"], (int)HttpStatusCode.BadRequest)
            });
        }

        // If both are present, treat a mismatch as suspicious.
        if (!string.IsNullOrWhiteSpace(context.RefreshTokenFromCookie) &&
            !string.IsNullOrWhiteSpace(context.RefreshTokenFromBody) &&
            !string.Equals(context.RefreshTokenFromCookie, context.RefreshTokenFromBody, StringComparison.Ordinal))
        {
            return Maybe.Create(new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Refresh token mismatch"], (int)HttpStatusCode.Unauthorized)
            });
        }

        context.RefreshToken = resolved;

        return await next(context, token);
    }
}
