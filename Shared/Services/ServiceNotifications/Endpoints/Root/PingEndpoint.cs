using FastSharp.Modules.Core;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Root;

public sealed class PingEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/ping/v1", () => Results.Json("pong"));
    }
}
