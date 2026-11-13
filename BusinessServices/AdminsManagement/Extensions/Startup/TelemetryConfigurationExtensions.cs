using snowcoreBlog.Backend.Infrastructure.Extensions;

namespace snowcoreBlog.Backend.AdminsManagement.Extensions.Startup;

public static class TelemetryConfigurationExtensions
{
    public static WebApplicationBuilder UseServiceDefaultsConfiguration(this WebApplicationBuilder builder)
    {
        builder.UseHostingConfiguration();

        return builder;
    }

    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddOpenTelemetry().ConnectBackendServices();

        return services;
    }
}