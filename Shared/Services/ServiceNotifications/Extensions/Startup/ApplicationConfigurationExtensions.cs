using FastSharp.Modules;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class ApplicationConfigurationExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseMiddlewareConfiguration()
           .UseApiAuthenticationConfiguration()
           .UseAntiforgeryOptInConfiguration()
           .UseOpenApiConfiguration();

        return app;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.MapFastSharpEndpoints();
    }

    public static async Task RunApplicationAsync(this WebApplication app)
    {
        await app.RunAsync();
    }
}