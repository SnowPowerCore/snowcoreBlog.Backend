using FastSharp.Modules.Core;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetActiveNotificationsEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(GetActiveNotificationsCachedStep)
    ])]
    protected GetActiveNotificationsDelegate GetActiveNotificationsCached { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/active/v1", async (GetActiveNotificationsDto request, CancellationToken ct) =>
            {
                var context = new GetActiveNotificationsContext(request.MaxCount);
                var result = await GetActiveNotificationsCached(context, ct);

                return new ApiResponseResult(
                    result?.ToApiResponse(),
                    result?.ToStatusCode() ?? StatusCodes.Status200OK);
            })
            .WithMetadata(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute { Duration = 300 }); // 5 minutes response cache
    }
}
