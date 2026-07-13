using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

public static class ApplicationConfigurationExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseMiddlewareConfiguration()
           .UseFastEndpointsConfiguration();

        return app;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();
    }

    public static async Task RunApplicationAsync(this WebApplication app)
    {
        await app.RunAsync();
    }
}