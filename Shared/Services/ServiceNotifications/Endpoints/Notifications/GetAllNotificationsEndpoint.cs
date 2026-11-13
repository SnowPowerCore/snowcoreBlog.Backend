using System.Net;
using FastSharp.Modules.Core;
using MaybeResults;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

// TODO: Add proper authorization for admin users
public sealed class GetAllNotificationsEndpoint(INotificationRepository notificationRepository) : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/v1", async (CancellationToken ct) =>
        {
            var notifications = await notificationRepository.GetAllAsync(ct);
            var dtos = notifications.ToDtos().ToList();
            var result = Maybe.Create<IReadOnlyList<NotificationDto>>(dtos);

            return new ApiResponseResult(
                result.ToApiResponse(),
                result.ToStatusCode());
        });
    }
}
