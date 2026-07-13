using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class TelemetryConfigurationExtensions
{
    public static WebApplicationBuilder UseServiceDefaultsConfiguration(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.AddServiceDefaults();

        return builder;
    }

    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services)
    {
        services.AddOpenTelemetry().ConnectBackendServices();

        return services;
    }
}