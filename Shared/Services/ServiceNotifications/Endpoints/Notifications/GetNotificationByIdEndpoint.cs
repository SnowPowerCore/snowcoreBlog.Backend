using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetNotificationByIdEndpoint(INotificationRepository notificationRepository) : EndpointWithoutRequest<ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        ResponseCache(300);
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Produces<ApiResponseForOpenApi<NotificationDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var notification = await notificationRepository.GetByIdAsync(id, ct);

        if (notification is default(NotificationEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Notification with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var result = Maybe.Create(notification.ToDto());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
