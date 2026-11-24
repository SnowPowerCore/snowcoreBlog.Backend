using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.ServiceNotifications.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class CreateNotificationEndpoint(INotificationRepository notificationRepository) : Endpoint<CreateNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Post(string.Empty);
        Version(1);
        SerializerContext(ServiceNotificationsSerializationContext.Default);
        Validator<CreateNotificationValidator>();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<CreateNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<CreateNotificationResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateNotificationDto req, CancellationToken ct)
    {
        var entity = new NotificationEntity
        {
            Title = req.Title,
            Description = req.Description,
            LinkUrl = req.LinkUrl,
            LinkText = req.LinkText,
            Type = req.Type.ToType(),
            Priority = req.Priority,
            IsActive = req.IsActive,
            IsDismissible = req.IsDismissible,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createdEntity = await notificationRepository.AddOrUpdateAsync(entity, token: ct);
        var result = Maybe.Create(new CreateNotificationResultDto { Id = createdEntity.Id });

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
