using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public class GetNotificationByIdEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required INotificationRepository NotificationRepository { get; set; }

    public override void Configure()
    {
        Get("{id:guid}");
        ResponseCache(300); // 5 minutes response cache
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Produces<ApiResponseForOpenApi<NotificationDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var notification = await NotificationRepository.GetByIdAsync(id, ct);

        if (notification is null)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Notification with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var dto = notification.ToDto();
        var result = Maybe.Create(dto);

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}