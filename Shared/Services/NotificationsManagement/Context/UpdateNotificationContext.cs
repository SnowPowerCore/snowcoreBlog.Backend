using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Context;

public class UpdateNotificationContext(UpdateNotificationDto request) : BaseGenericContext
{
    public UpdateNotificationDto UpdateRequest { get; } = request;
}
