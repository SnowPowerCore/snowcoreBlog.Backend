using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class DeleteNotificationEndpoint(INotificationRepository notificationRepository) : Endpoint<DeleteNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Delete(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<DeleteNotificationValidator>();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<DeleteNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<DeleteNotificationResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(DeleteNotificationDto req, CancellationToken ct)
    {
        var existingEntity = await notificationRepository.GetByIdAsync(req.Id, ct);

        if (existingEntity is default(NotificationEntity))
        {
            var notFound = NotificationNotFoundError<DeleteNotificationResultDto>.Create(
                $"Notification with ID '{req.Id}' was not found.");

            await Send.ResponseAsync(
                notFound.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
                notFound.ToStatusCode(),
                ct);
            return;
        }

        await notificationRepository.RemoveAsync(existingEntity!, token: ct);
        var result = Maybe.Create(new DeleteNotificationResultDto { Success = true });

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
