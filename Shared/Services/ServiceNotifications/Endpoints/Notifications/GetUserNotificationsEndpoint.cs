using FastSharp.Modules.Core;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Get;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class GetUserNotificationsEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(GetActiveNotificationsForUserStep)
    ])]
    protected GetActiveNotificationsDelegate GetUserNotifications { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/active/user/v1", async (GetActiveNotificationsDto request, CancellationToken ct) =>
            {
                var context = new GetActiveNotificationsContext(request.MaxCount);
                var result = await GetUserNotifications(context, ct);

                return new ApiResponseResult(
                    result?.ToApiResponse(),
                    result?.ToStatusCode() ?? StatusCodes.Status200OK);
            })
            .WithMetadata(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute { Duration = 900 }) // 15 minutes response cache
            .RequireAuthorization(static policy =>
                policy.RequireClaim(ReaderAccountClaimConstants.ReaderAccountReaderAccountClaimKey));
    }
}
