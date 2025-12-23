using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Gateway.Middleware.Middleware;

public class ApiAccessRestrictionsMiddleware(IApiAccessRestrictionsSnapshotStore snapshotStore,
                                             ILocalApiAccessRestrictionsEvaluator evaluator,
                                             ILogger<ApiAccessRestrictionsMiddleware> logger) : IMiddleware
{
    private static readonly JsonSerializerOptions _json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    }.SetJsonSerializationContext();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;

        // Fail-open: if anything goes wrong, do not block traffic.
        CheckApiAccessResponseDto? decision = null;
        try
        {
            var compiled = snapshotStore.CurrentCompiled;
            if (compiled is not default(CompiledApiAccessRestrictionsSnapshot))
            {
                decision = evaluator.Evaluate(compiled, new()
                {
                    Path = request.Path.ToString(),
                    Method = request.Method,
                    Ip = GetClientIp(context)?.ToString(),
                    CountryCode = GetCountryCode(request),
                    Tags = []
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "ApiAccessRestrictions local evaluation failed; allowing request.");
        }

        if (decision is default(CheckApiAccessResponseDto) || decision.IsAllowed)
        {
            await next(context);
            return;
        }

        if (context.Response.HasStarted)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = decision.StatusCode ?? StatusCodes.Status403Forbidden;
        context.Response.ContentType = decision.ContentType ?? "application/json";

        if (!string.IsNullOrWhiteSpace(decision.BodyJson))
        {
            await context.Response.WriteAsync(decision.BodyJson, context.RequestAborted);
            return;
        }

        var reason = decision.Reason ?? "Access restricted by policy.";
        var apiResponse = new ApiResponse(
            Data: default,
            DataCount: 0,
            StatusCode: context.Response.StatusCode,
            Errors: [reason]);

        await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, _json), context.RequestAborted);
    }

    private static IPAddress? GetClientIp(HttpContext context)
    {
        // Forwarded headers run after this middleware in current pipeline, so parse manually.
        var xff = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrWhiteSpace(xff))
        {
            var first = xff.Split(',')[0].Trim();
            if (IPAddress.TryParse(first, out var ip))
                return ip;
        }

        return context.Connection.RemoteIpAddress;
    }

    private static string? GetCountryCode(HttpRequest request)
    {
        // Prefer common CDN headers; can be extended later.
        var cf = request.Headers["CF-IPCountry"].ToString();
        if (!string.IsNullOrWhiteSpace(cf))
            return cf;

        var cloudFront = request.Headers["CloudFront-Viewer-Country"].ToString();
        if (!string.IsNullOrWhiteSpace(cloudFront))
            return cloudFront;

        var custom = request.Headers["X-Country-Code"].ToString();
        if (!string.IsNullOrWhiteSpace(custom))
            return custom;

        return string.Empty;
    }
}