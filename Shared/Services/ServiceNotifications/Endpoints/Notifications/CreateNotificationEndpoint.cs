using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Create;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public class CreateNotificationEndpoint : Endpoint<CreateNotificationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(CreateNotificationEntityStep),
    ])]
    protected CreateNotificationDelegate CreateNotification { get; }

    public override void Configure()
    {
        Post(string.Empty);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<CreateNotificationValidator>();
        AllowAnonymous(); // TODO: Add proper authorization for admin users
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<CreateNotificationDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<CreateNotificationResultDto>>((int)HttpStatusCode.Created, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateNotificationDto req, CancellationToken ct)
    {
        var context = new CreateNotificationContext(req);

        var result = await CreateNotification(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}