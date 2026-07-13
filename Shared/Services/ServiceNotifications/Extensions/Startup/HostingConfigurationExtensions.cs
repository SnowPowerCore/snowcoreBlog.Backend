using System.Text.Json.Serialization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing.Constraints;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class HostingConfigurationExtensions
{
    public static WebApplicationBuilder UseHostingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider(static (c, options) =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

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

        services.Configure<CookiePolicyOptions>(static options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return services;
    }
}