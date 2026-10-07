using ActualLab.Fusion;
using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.Blazor.Authentication;
using ActualLab.Fusion.Server;
using ActualLab.Rpc;
using ActualLab.Rpc.Server;
using snowcoreBlog.Backend.AuthorsManagement.Services;
using snowcoreBlog.PublicApi.Services;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

/// <summary>
/// Wires up ActualLab.Fusion for the AuthorsManagement backend:
/// <list type="bullet">
/// <item>Registers the Fusion compute-service host and the RPC WebSocket server.</item>
/// <item>Exposes <see cref="IAuthorDisplayNameAvailabilityService"/> as a server-side compute service
/// (transparently replicated to Blazor clients via ActualLab.Rpc).</item>
/// <item>Maps the RPC WebSocket endpoint used by Fusion clients.</item>
/// </list>
/// </summary>
public static class FusionConfigurationExtensions
{
    /// <summary>
    /// WebSocket path the Fusion client connects to (after the YARP
    /// <c>/api/authors</c> prefix strip). Matches the YARP route
    /// <c>backend-authorsmanagement-fusion</c>.
    /// </summary>
    public const string RpcWebSocketPath = "/fusion/ws";

    public static IServiceCollection AddFusionConfiguration(this IServiceCollection services)
    {
        // Override the default public WebSocket path before any RpcWebSocketServer is built.
        // RpcWebSocketServerOptions is a record with init-only members, so we replace the
        // static Default rather than mutating it via IOptions.
        RpcWebSocketServerOptions.Default = RpcWebSocketServerOptions.Default with {
            RequestPath = RpcWebSocketPath,
        };

        var fusion = services.AddFusion(RpcServiceMode.Server, true);
        fusion.AddWebServer();

        // Add your Fusion compute services as servers
        fusion.AddServer<IAuthorDisplayNameAvailabilityService, AuthorDisplayNameAvailabilityService>();

        fusion.AddBlazor().AddAuthentication().AddPresenceReporter();

        return services;
    }

    public static WebApplication UseFusionConfiguration(this WebApplication app)
    {
        // Fusion RPC runs over WebSockets — enable the ASP.NET Core WebSocket middleware.
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30),
        });

        app.UseFusionSession();
        app.MapRpcWebSocketServer();
        return app;
    }
}
