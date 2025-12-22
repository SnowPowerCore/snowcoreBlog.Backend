using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.ErrorResults;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Update;

public class ValidateNotificationExistsStep(INotificationRepository notificationRepository) 
    : IStep<UpdateNotificationDelegate, UpdateNotificationContext, IMaybe<NotificationDto>>
{
    public async Task<IMaybe<NotificationDto>> InvokeAsync(
        UpdateNotificationContext context, 
        UpdateNotificationDelegate next, 
        CancellationToken token = default)
    {
        var existingNotification = await notificationRepository.GetByIdAsync(context.UpdateRequest.Id, token);

        if (existingNotification is null)
        {
            return NotificationNotFoundError<NotificationDto>.Create(
                $"Notification with ID '{context.UpdateRequest.Id}' was not found.");
        }

        return await next(context, token);
    }
}