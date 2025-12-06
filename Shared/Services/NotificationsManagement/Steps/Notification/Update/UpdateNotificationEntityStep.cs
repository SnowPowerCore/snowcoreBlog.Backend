using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.Backend.NotificationsManagement.Delegates;
using snowcoreBlog.Backend.NotificationsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.NotificationsManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Steps.Notification.Update;

public class UpdateNotificationEntityStep(INotificationRepository notificationRepository) 
    : IStep<UpdateNotificationDelegate, UpdateNotificationContext, IMaybe<NotificationDto>>
{
    public async Task<IMaybe<NotificationDto>> InvokeAsync(
        UpdateNotificationContext context, 
        UpdateNotificationDelegate next, 
        CancellationToken token = default)
    {
        var existingEntity = await notificationRepository.GetByIdAsync(context.UpdateRequest.Id, token);

        var updatedEntity = existingEntity! with
        {
            Title = context.UpdateRequest.Title,
            Description = context.UpdateRequest.Description,
            LinkUrl = context.UpdateRequest.LinkUrl,
            LinkText = context.UpdateRequest.LinkText,
            Type = context.UpdateRequest.Type.ToType(),
            Priority = context.UpdateRequest.Priority,
            IsActive = context.UpdateRequest.IsActive,
            IsDismissible = context.UpdateRequest.IsDismissible,
            StartDate = context.UpdateRequest.StartDate,
            EndDate = context.UpdateRequest.EndDate,
            ModifiedAt = DateTime.UtcNow
        };

        var savedEntity = await notificationRepository.AddOrUpdateAsync(updatedEntity, updatedEntity.Id, token: token);

        return Maybe.Create(savedEntity.ToDto());
    }
}
