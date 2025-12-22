using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Update;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public class UpdateNotificationEndpoint : Endpoint<UpdateNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateNotificationExistsStep),
        typeof(UpdateNotificationEntityStep),
    ])]
    protected UpdateNotificationDelegate UpdateNotification { get; }

    public override void Configure()
    {
        Put(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<UpdateNotificationValidator>();
        AllowAnonymous(); // TODO: Add proper authorization for admin users
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<UpdateNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<NotificationDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(UpdateNotificationDto req, CancellationToken ct)
    {
        var context = new UpdateNotificationContext(req);

        var result = await UpdateNotification(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}