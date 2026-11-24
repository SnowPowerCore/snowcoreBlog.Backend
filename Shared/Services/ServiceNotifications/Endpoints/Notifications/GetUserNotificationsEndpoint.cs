using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Shared.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetUserNotificationsEndpoint : Endpoint<GetActiveNotificationsDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(GetActiveNotificationsForUserStep)
    ])]
    protected GetActiveNotificationsDelegate GetUserNotifications { get; } = default!;

    public override void Configure()
    {
        Get("active/user");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        ResponseCache(900); // 15 minutes response cache
        Claims(ReaderAccountClaimConstants.ReaderAccountReaderAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.TechnicalNotifications)
            .Produces<ApiResponseForOpenApi<List<NotificationDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(GetActiveNotificationsDto req, CancellationToken ct)
    {
        var context = new GetActiveNotificationsContext(req.MaxCount);
        var result = await GetUserNotifications(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.OK,
            ct);
    }
}
