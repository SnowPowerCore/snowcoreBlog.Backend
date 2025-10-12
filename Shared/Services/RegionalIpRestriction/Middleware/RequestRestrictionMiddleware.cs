using System.Net;
using snowcoreBlog.Backend.RegionalIpRestriction.Services;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Middleware;

public class RequestRestrictionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestRestrictionService _service;

    public RequestRestrictionMiddleware(RequestDelegate next, IRequestRestrictionService service)
    {
        _next = next;
        _service = service;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress ?? IPAddress.Loopback;
        var path = context.Request.Path.ToString();

        // var allowed = await _service.IsAllowedAsync(ip, path, context.RequestAborted);
        // if (!allowed)
        // {
        //     context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //     await context.Response.WriteAsync("Access restricted by IP/Region policy.");
        //     return;
        // }

        await _next(context);
    }
}