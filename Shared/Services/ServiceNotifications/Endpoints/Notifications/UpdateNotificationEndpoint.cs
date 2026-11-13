using FastSharp.Modules.Core;
using FluentValidation;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Update;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class UpdateNotificationEndpoint(UpdateNotificationValidator validator) : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(ValidateNotificationExistsStep),
        typeof(UpdateNotificationEntityStep),
    ])]
    protected UpdateNotificationDelegate UpdateNotification { get; }

    public void Map(RouteGroupBuilder app)
    {
        // TODO: Add proper authorization for admin users
        app.MapPut("/v1", async (UpdateNotificationDto request, CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(request, ct);
                if (!validationResult.IsValid)
                {
                    var payload = EndpointResponseHelpers.ApiResponseFromFailures(validationResult.Errors);
                    return new ApiResponseResult(payload, StatusCodes.Status400BadRequest);
                }

                var context = new UpdateNotificationContext(request);
                var result = await UpdateNotification(context, ct);

                return new ApiResponseResult(
                    result?.ToApiResponse(),
                    result?.ToStatusCode() ?? StatusCodes.Status500InternalServerError);
            })
            .WithMetadata(new RequireAntiforgeryMetadata());
    }
}
