using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Context;

public class UpdateNotificationContext(UpdateNotificationDto request) : BaseGenericContext
{
    public UpdateNotificationDto UpdateRequest { get; } = request;
}