using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing.Constraints;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class HostingConfigurationExtensions
{
    public static WebApplicationBuilder UseHostingConfiguration(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.AddServiceDefaults();

        return builder;
    }

    public static IServiceCollection AddHostingConfiguration(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(static options =>
        {
            options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
        });

        services.ConfigureHttpJsonOptions(static options =>
        {
            // Wire parity with the FastEndpoints serializer: camelCase properties,
            // enums as strings, numbers readable from strings, shared source-gen
            // context first in the resolver chain.
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.SetJsonSerializationContext();
        });

        return services;
    }
}