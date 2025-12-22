using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.ErrorResults;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Delete;

public class ValidateNotificationExistsForDeleteStep(INotificationRepository notificationRepository) 
    : IStep<DeleteNotificationDelegate, DeleteNotificationContext, IMaybe<DeleteNotificationResultDto>>
{
    public async Task<IMaybe<DeleteNotificationResultDto>> InvokeAsync(
        DeleteNotificationContext context, 
        DeleteNotificationDelegate next, 
        CancellationToken token = default)
    {
        var existingNotification = await notificationRepository.GetByIdAsync(context.DeleteRequest.Id, token);

        if (existingNotification is null)
        {
            return NotificationNotFoundError<DeleteNotificationResultDto>.Create(
                $"Notification with ID '{context.DeleteRequest.Id}' was not found.");
        }

        return await next(context, token);
    }
}