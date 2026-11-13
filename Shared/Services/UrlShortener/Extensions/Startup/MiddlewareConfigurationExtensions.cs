using System.Net.Mime;
using FastEndpoints;
using MinimalStepifiedSystem.Extensions;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class MiddlewareConfigurationExtensions
{
    public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
    {
        app.UseHttpsRedirection()
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }

    public static WebApplication UseCustomMiddlewareConfiguration(this WebApplication app)
    {
        app.UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json]);

        return app;
    }
}