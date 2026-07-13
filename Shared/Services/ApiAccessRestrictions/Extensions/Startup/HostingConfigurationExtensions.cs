using Microsoft.AspNetCore.Routing.Constraints;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using System.Text.Json.Serialization;

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
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.SetJsonSerializationContext();
        });

        return services;
    }
}