using MinimalStepifiedSystem.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class ApplicationConfigurationExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseMiddlewareConfiguration()
           .UseLocalizationConfiguration()
           .UseApiAuthenticationConfiguration()
           .UseFastEndpointsConfiguration()
           .UseOpenApiConfiguration();

        // Registers the static service-provider supplier used by the MinimalStepifiedSystem
        // AspectInjector property aspects (stepified delegates) on first property access.
        app.UseStepifiedSystem();

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