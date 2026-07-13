using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class HostingConfigurationExtensions
{
    public static WebApplicationBuilder UseHostingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider(static (c, opts) =>
        {
            opts.ValidateScopes = true;
            opts.ValidateOnBuild = true;
        });

        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.AddServiceDefaults();

        return builder;
    }

    public static IServiceCollection AddHostingConfiguration(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.ConfigureHttpJsonOptions(static options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.SetJsonSerializationContext();
        });

        return services;
    }
}