using System.Net;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Middleware;

public class RequestRestrictionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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

        await next(context);
    }
}