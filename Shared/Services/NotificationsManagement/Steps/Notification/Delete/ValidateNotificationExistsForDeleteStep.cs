using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.Backend.NotificationsManagement.Delegates;
using snowcoreBlog.Backend.NotificationsManagement.ErrorResults;
using snowcoreBlog.Backend.NotificationsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Steps.Notification.Delete;

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