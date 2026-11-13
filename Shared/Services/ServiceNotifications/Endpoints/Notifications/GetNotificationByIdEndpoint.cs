using System.Net;
using FastSharp.Modules.Core;
using MaybeResults;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ServiceNotifications.Endpoints;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetNotificationByIdEndpoint(INotificationRepository notificationRepository) : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/{id:guid}/v1", async (Guid id, CancellationToken ct) =>
            {
                var notification = await notificationRepository.GetByIdAsync(id, ct);

                if (notification is default(Core.Entities.Notification.NotificationEntity))
                {
                    return new ApiResponseResult(
                        ErrorResponseUtilities.ApiResponseWithErrors(
                            [$"Notification with ID '{id}' was not found."],
                            (int)HttpStatusCode.NotFound),
                        (int)HttpStatusCode.NotFound);
                }

                var dto = notification.ToDto();
                var result = Maybe.Create(dto);

                return new ApiResponseResult(
                    result.ToApiResponse(),
                    result.ToStatusCode());
            })
            .WithMetadata(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute { Duration = 300 }); // 5 minutes response cache
    }
}
