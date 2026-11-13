using FastSharp.Modules.Core;
using FluentValidation;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Create;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Notifications;

public sealed class CreateNotificationEndpoint(CreateNotificationValidator validator) : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(CreateNotificationEntityStep),
    ])]
    protected CreateNotificationDelegate CreateNotification { get; }

    public void Map(RouteGroupBuilder app)
    {
        // TODO: Add proper authorization for admin users
        app.MapPost("/v1", async (CreateNotificationDto request, CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(request, ct);
                if (!validationResult.IsValid)
                {
                    var payload = EndpointResponseHelpers.ApiResponseFromFailures(validationResult.Errors);
                    return new ApiResponseResult(payload, StatusCodes.Status400BadRequest);
                }

                var context = new CreateNotificationContext(request);
                var result = await CreateNotification(context, ct);

                return new ApiResponseResult(
                    result?.ToApiResponse(),
                    result?.ToStatusCode() ?? StatusCodes.Status500InternalServerError);
            })
            .WithMetadata(new RequireAntiforgeryMetadata());
    }
}
