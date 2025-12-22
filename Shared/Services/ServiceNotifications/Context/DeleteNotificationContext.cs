using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Context;

public class DeleteNotificationContext(DeleteNotificationDto request) : BaseGenericContext
{
    public DeleteNotificationDto DeleteRequest { get; } = request;
}