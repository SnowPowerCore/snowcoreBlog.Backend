using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.ServiceNotifications.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.ErrorResults;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class UpdateNotificationEndpoint(INotificationRepository notificationRepository) : Endpoint<UpdateNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Put(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<UpdateNotificationValidator>();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<UpdateNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<NotificationDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(UpdateNotificationDto req, CancellationToken ct)
    {
        var existingEntity = await notificationRepository.GetByIdAsync(req.Id, ct);

        if (existingEntity is default(NotificationEntity))
        {
            var notFound = NotificationNotFoundError<NotificationDto>.Create(
                $"Notification with ID '{req.Id}' was not found.");

            await Send.ResponseAsync(
                notFound.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
                notFound.ToStatusCode(),
                ct);
            return;
        }

        var updatedEntity = existingEntity! with
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
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var savedEntity = await notificationRepository.AddOrUpdateAsync(updatedEntity, updatedEntity.Id, token: ct);
        var result = Maybe.Create(savedEntity.ToDto());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
