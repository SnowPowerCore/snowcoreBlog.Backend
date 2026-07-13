using FastEndpoints;
using FastEndpoints.Swagger;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;
    private static readonly JsonStringEnumConverter JsonStringEnumConverter = new();

    public static IServiceCollection AddFastEndpointsConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization()
                .AddFastEndpoints(static options =>
                {
                    options.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
                })
                .SwaggerDocument(options =>
                {
                    options.AutoTagPathSegmentIndex = 0;
                    options.ShortSchemaNames = true;
                    options.MaxEndpointVersion = GlobalVersion;
                    options.SerializerSettings = s =>
                    {
                        s.Converters.Add(JsonStringEnumConverter);
                        s.SetJsonSerializationContext();
                        s.PropertyNamingPolicy = null;
                    };
                });

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseAuthorization()
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