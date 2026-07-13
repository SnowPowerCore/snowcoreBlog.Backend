using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Extensions;
using System.Net.Mime;
using System.Text.Json;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

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
        app.UseMiddleware<UserCookieJsonWebTokenMiddleware>()
            .UseAuthentication()
            .UseAuthorization()
            .UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json])
            .UseFastEndpointsDiagnosticsMiddleware()
            .UseFastEndpoints(c =>
            {
                c.Endpoints.NameGenerator = static ctx =>
                {
                    var currentName = ctx.EndpointType.Name;
                    return currentName.TrimEnd("Endpoint");
                };
                c.Endpoints.ShortNames = true;
                c.Endpoints.RoutePrefix = default;
                c.Versioning.Prefix = "v";
                c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                c.Serializer.Options.SetJsonSerializationContext();
            });

        return app;
    }
}