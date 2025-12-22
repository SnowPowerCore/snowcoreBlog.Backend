using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Context;

public class CreateNotificationContext(CreateNotificationDto request) : BaseGenericContext
{
    public CreateNotificationDto CreateRequest { get; } = request;
    
    public Guid? CreatedByUserId { get; set; }
}