using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.Backend.Core.Constants;

namespace snowcoreBlog.Backend.AspireYarpGateway.Middleware;

public class UserCookieJsonWebTokenMiddleware : IMiddleware
{
    private const string XContentTypeOptionsHeader = "X-Content-Type-Options";
    private const string XContentTypeOptionsHeaderValue = "nosniff";
    private const string XXSSProtectionHeader = "X-Xss-Protection";
    private const string XXSSProtectionHeaderValue = "1";
    private const string XFrameOptionsHeader = "X-Frame-Options";
    private const string XFrameOptionsHeaderValue = "DENY";
    private const string AuthorizationHeader = "Authorization";

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Append(XContentTypeOptionsHeader, XContentTypeOptionsHeaderValue);
        context.Response.Headers.Append(XXSSProtectionHeader, XXSSProtectionHeaderValue);
        context.Response.Headers.Append(XFrameOptionsHeader, XFrameOptionsHeaderValue);

        var token = context.Request.Cookies[AuthCookieConstants.UserAccessTokenCookieName];
        if (!string.IsNullOrEmpty(token))
            context.Request.Headers.Append(AuthorizationHeader, $"{JwtBearerDefaults.AuthenticationScheme} {token}");

        return next(context);
    }
}