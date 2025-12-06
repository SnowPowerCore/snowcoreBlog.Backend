using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.Backend.NotificationsManagement.Delegates;
using snowcoreBlog.Backend.NotificationsManagement.Steps.Notification.Get;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.NotificationsManagement.Endpoints.Notifications;

public class GetActiveNotificationsEndpoint : Endpoint<GetActiveNotificationsDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(GetActiveNotificationsCachedStep)
    ])]
    protected GetActiveNotificationsDelegate GetActiveNotificationsCached { get; } = default!;

    public override void Configure()
    {
        Get("active");
        ResponseCache(300); // 5 minutes response cache
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Accepts<GetActiveNotificationsDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<IEnumerable<NotificationDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(GetActiveNotificationsDto req, CancellationToken ct)
    {
        var context = new GetActiveNotificationsContext(req.MaxCount);
        var result = await GetActiveNotificationsCached(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.OK,
            ct);
    }
}
