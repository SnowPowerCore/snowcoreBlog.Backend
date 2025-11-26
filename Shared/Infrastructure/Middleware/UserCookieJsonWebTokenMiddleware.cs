using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using snowcoreBlog.Backend.Core.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Middleware;

public class UserCookieJsonWebTokenMiddleware : IMiddleware
{
    private const string XContentTypeOptionsHeaderValue = "nosniff";
    private const string XXSSProtectionHeaderValue = "1";
    private const string XFrameOptionsHeaderValue = "DENY";

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Append(HeaderNames.XContentTypeOptions, XContentTypeOptionsHeaderValue);
        context.Response.Headers.Append(HeaderNames.XXSSProtection, XXSSProtectionHeaderValue);
        context.Response.Headers.Append(HeaderNames.XFrameOptions, XFrameOptionsHeaderValue);

        var token = context.Request.Cookies[AuthCookieConstants.UserAccessTokenCookieName];
        if (!string.IsNullOrWhiteSpace(token))
            context.Request.Headers.Append(HeaderNames.Authorization, $"{JwtBearerDefaults.AuthenticationScheme} {token}");

        return next(context);
    }
}