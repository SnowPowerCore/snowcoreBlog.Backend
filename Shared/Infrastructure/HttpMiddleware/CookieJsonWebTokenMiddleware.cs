using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.Backend.IAM.Core.Constants;

namespace snowcoreBlog.Backend.Infrastructure.HttpMiddleware;

public class CookieJsonWebTokenMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Xss-Protection", "1");
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        var token = context.Request.Cookies[AuthConstants.TokenCookieName];
        if (!string.IsNullOrEmpty(token))
            context.Request.Headers.Append("Authorization", $"{JwtBearerDefaults.AuthenticationScheme} {token}");

        return next(context);
    }
}