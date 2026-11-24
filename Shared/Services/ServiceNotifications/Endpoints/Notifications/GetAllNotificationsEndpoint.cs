using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetAllNotificationsEndpoint(INotificationRepository notificationRepository) : EndpointWithoutRequest<ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get(string.Empty);
        Version(1);
        SerializerContext(ServiceNotificationsSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Produces<ApiResponseForOpenApi<List<NotificationDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var notifications = await notificationRepository.GetAllAsync(ct);
        var result = Maybe.Create(notifications.ToDtos().ToList());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
