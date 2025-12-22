using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ServiceNotifications.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Create;

public class CreateNotificationEntityStep(INotificationRepository notificationRepository) 
    : IStep<CreateNotificationDelegate, CreateNotificationContext, IMaybe<CreateNotificationResultDto>>
{
    public async Task<IMaybe<CreateNotificationResultDto>> InvokeAsync(
        CreateNotificationContext context, 
        CreateNotificationDelegate next, 
        CancellationToken token = default)
    {
        var entity = new NotificationEntity
        {
            Title = context.CreateRequest.Title,
            Description = context.CreateRequest.Description,
            LinkUrl = context.CreateRequest.LinkUrl,
            LinkText = context.CreateRequest.LinkText,
            Type = context.CreateRequest.Type.ToType(),
            Priority = context.CreateRequest.Priority,
            IsActive = context.CreateRequest.IsActive,
            IsDismissible = context.CreateRequest.IsDismissible,
            StartDate = context.CreateRequest.StartDate,
            EndDate = context.CreateRequest.EndDate,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = context.CreatedByUserId
        };

        var createdEntity = await notificationRepository.AddOrUpdateAsync(entity, token: token);

        return Maybe.Create(new CreateNotificationResultDto { Id = createdEntity.Id });
    }
}