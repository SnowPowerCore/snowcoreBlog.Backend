using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Context;

public class DeleteNotificationContext(DeleteNotificationDto request) : BaseGenericContext
{
    public DeleteNotificationDto DeleteRequest { get; } = request;
}