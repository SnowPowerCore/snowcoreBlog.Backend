using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Delete;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public class DeleteNotificationEndpoint : Endpoint<DeleteNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateNotificationExistsForDeleteStep),
        typeof(DeleteNotificationEntityStep),
    ])]
    protected DeleteNotificationDelegate DeleteNotification { get; }

    public override void Configure()
    {
        Delete(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<DeleteNotificationValidator>();
        AllowAnonymous(); // TODO: Add proper authorization for admin users
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<DeleteNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<DeleteNotificationResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(DeleteNotificationDto req, CancellationToken ct)
    {
        var context = new DeleteNotificationContext(req);

        var result = await DeleteNotification(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}