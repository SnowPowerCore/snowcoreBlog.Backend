using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ServiceNotifications.Repositories.Marten;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.RegisterDocumentType<NotificationEntity>();
            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}