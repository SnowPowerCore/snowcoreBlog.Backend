using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.Backend.ServiceNotifications.Delegates;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Steps.Notification.Delete;

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