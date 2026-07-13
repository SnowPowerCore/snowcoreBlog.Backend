using System.Text.Json;
using FastEndpoints;
using snowcoreBlog.Backend.Infrastructure.Extensions;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;

    public static IServiceCollection AddFastEndpointsConfiguration(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddAuthorization()
            .AddAntiforgery()
            .AddFastEndpoints();

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.ShortNames = true;
            c.Endpoints.RoutePrefix = default;
            c.Versioning.Prefix = "v";
            c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            c.Serializer.Options.SetJsonSerializationContext();
        });

        return app;
    }
}