using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ServiceNotifications.Repositories.Marten;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Create;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Delete;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Update;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Steps
        services.AddScoped<GetActiveNotificationsCachedStep>();
        services.AddScoped<GetActiveNotificationsForUserStep>();
        services.AddScoped<CreateNotificationEntityStep>();
        services.AddScoped<ValidateNotificationExistsStep>();
        services.AddScoped<UpdateNotificationEntityStep>();
        services.AddScoped<ValidateNotificationExistsForDeleteStep>();
        services.AddScoped<DeleteNotificationEntityStep>();

        return services;
    }
}