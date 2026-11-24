using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Steps
        services.AddScoped<GetActiveNotificationsCachedStep>();
        services.AddScoped<GetActiveNotificationsForUserStep>();

        return services;
    }
}