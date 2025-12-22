using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.Backend.NotificationsManagement.Delegates;
using snowcoreBlog.Backend.NotificationsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Steps.Notification.Delete;

public class DeleteNotificationEntityStep(INotificationRepository notificationRepository) 
    : IStep<DeleteNotificationDelegate, DeleteNotificationContext, IMaybe<DeleteNotificationResultDto>>
{
    public async Task<IMaybe<DeleteNotificationResultDto>> InvokeAsync(
        DeleteNotificationContext context, 
        DeleteNotificationDelegate next, 
        CancellationToken token = default)
    {
        var existingEntity = await notificationRepository.GetByIdAsync(context.DeleteRequest.Id, token);

        await notificationRepository.RemoveAsync(existingEntity!, token: token);

        return Maybe.Create(new DeleteNotificationResultDto { Success = true });
    }
}