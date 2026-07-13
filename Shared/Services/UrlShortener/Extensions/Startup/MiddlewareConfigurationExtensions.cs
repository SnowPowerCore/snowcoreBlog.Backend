using System.Net.Mime;
using FastEndpoints;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class MiddlewareConfigurationExtensions
{
    public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
    {
        app.UseHttpsRedirection()
            .UseAuthentication()
            .UseAuthorization()
            .UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json])
            .UseFastEndpoints();

        return app;
    }
}