using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using FastEndpoints.Swagger;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using snowcoreBlog.Backend.Infrastructure.Processors;
using snowcoreBlog.PublicApi.Extensions;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;
    private static readonly JsonStringEnumConverter JsonStringEnumConverter = new();

    public static IServiceCollection AddFastEndpointsConfiguration(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddAuthorization()
                .AddAntiforgery()
                .AddFastEndpoints(static options =>
                {
                    options.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
                })
                .SwaggerDocument(options =>
                {
                    options.AutoTagPathSegmentIndex = 0;
                    options.ShortSchemaNames = true;
                    options.MaxEndpointVersion = GlobalVersion;
                    options.DocumentSettings = static s =>
                    {
                        s.DocumentName = $"v{GlobalVersion}";
                        s.Version = $"v{GlobalVersion}";
                        s.SchemaSettings.IgnoreObsoleteProperties = true;
                        s.OperationProcessors.Add(new AntiforgeryHeaderProcessor());
                        s.OperationProcessors.Add(new AltchaHeaderProcessor());
                    };
                    options.SerializerSettings = s =>
                    {
                        s.Converters.Add(JsonStringEnumConverter);
                        s.SetJsonSerializationContext();
                        s.PropertyNamingPolicy = null;
                        s.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    };
                })
                .AddResponseCaching();

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseResponseCaching()
            .UseMiddleware<UserCookieJsonWebTokenMiddleware>()
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
                c.Serializer.Options.Converters.Add(JsonStringEnumConverter);
                c.Serializer.Options.SetJsonSerializationContext();
            });

        return app;
    }
}