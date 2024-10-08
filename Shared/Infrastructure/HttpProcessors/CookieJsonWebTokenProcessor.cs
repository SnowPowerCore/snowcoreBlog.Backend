using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.Backend.IAM.Core.Constants;

namespace snowcoreBlog.Backend.Infrastructure.HttpProcessors;

public class CookieJsonWebTokenProcessor : IGlobalPreProcessor
{
    private const string XContentTypeOptionsHeader = "X-Content-Type-Options";
    private const string XContentTypeOptionsHeaderValue = "nosniff";
    private const string XXSSProtectionHeader = "X-Xss-Protection";
    private const string XXSSProtectionHeaderValue = "1";
    private const string XFrameOptionsHeader = "X-Frame-Options";
    private const string XFrameOptionsHeaderValue = "DENY";
    private const string AuthorizationHeader = "Authorization";

    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        context.HttpContext.Response.Headers.Append(XContentTypeOptionsHeader, XContentTypeOptionsHeaderValue);
        context.HttpContext.Response.Headers.Append(XXSSProtectionHeader, XXSSProtectionHeaderValue);
        context.HttpContext.Response.Headers.Append(XFrameOptionsHeader, XFrameOptionsHeaderValue);

        var token = context.HttpContext.Request.Cookies[AuthConstants.TokenCookieName];
        if (!string.IsNullOrEmpty(token))
            context.HttpContext.Request.Headers.Append(AuthorizationHeader, $"{JwtBearerDefaults.AuthenticationScheme} {token}");

        return Task.CompletedTask;
    }
}