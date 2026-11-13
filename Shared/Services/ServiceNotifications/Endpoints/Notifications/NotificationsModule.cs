using FastSharp.Modules.Configuration;
using FastSharp.Modules.Core;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class NotificationsModule : Module
{
    public NotificationsModule()
    {
        ConfigureModule("/notifications", static options => options
            .WithTags("Notifications"));

        Include<CreateNotificationEndpoint>();
        Include<GetNotificationByIdEndpoint>();
        Include<UpdateNotificationEndpoint>();
        Include<DeleteNotificationEndpoint>();
        Include<GetAllNotificationsEndpoint>();
        Include<GetActiveNotificationsEndpoint>();
        Include<GetUserNotificationsEndpoint>();
    }
}
