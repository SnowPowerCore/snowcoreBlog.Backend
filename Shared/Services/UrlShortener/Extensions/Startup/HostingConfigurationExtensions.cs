using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing.Constraints;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.UrlShortener.Options;
using snowcoreBlog.ServiceDefaults.Extensions;

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

        return builder;
    }

    public static IServiceCollection AddHostingConfiguration(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(static options =>
        {
            options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
        });

        services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        services.ConfigureHttpJsonOptions(static options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.SetJsonSerializationContext(UrlShortenerSerializationContext.Default);
        });

        return services;
    }

    public static IServiceCollection AddUrlShortenerOptionsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "UrlShortener")
    {
        services.Configure<UrlShortenerOptions>(configuration.GetSection(section));

        return services;
    }
}