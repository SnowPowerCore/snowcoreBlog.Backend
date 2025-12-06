using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.NotificationsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.NotificationsManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.NotificationsManagement.Endpoints.Notifications;

public class GetAllNotificationsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required INotificationRepository NotificationRepository { get; set; }

    public override void Configure()
    {
        Get(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous(); // TODO: Add proper authorization for admin users
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Produces<ApiResponseForOpenApi<IEnumerable<NotificationDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var notifications = await NotificationRepository.GetAllAsync(ct);
        var dtos = notifications.ToDtos().ToList();
        var result = Maybe.Create<IReadOnlyList<NotificationDto>>(dtos);

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
