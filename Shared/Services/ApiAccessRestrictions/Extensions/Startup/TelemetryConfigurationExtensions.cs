using snowcoreBlog.Backend.Infrastructure.Extensions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class TelemetryConfigurationExtensions
{
    public static WebApplicationBuilder UseServiceDefaultsConfiguration(this WebApplicationBuilder builder)
    {
        builder.UseHostingConfiguration();

        return builder;
    }

    public static IServiceCollection AddTelemetryConfiguration(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddOpenTelemetry().ConnectBackendServices();

        return services;
    }
}