using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions;

[Mapper]
public static partial class NotificationMapper
{
    public static partial NotificationDto ToDto(this NotificationEntity entity);

    public static partial IEnumerable<NotificationDto> ToDtos(this IEnumerable<NotificationEntity> entities);

    public static partial NotificationEntity ToEntity(this CreateNotificationDto dto);

    public static partial NotificationEntity ToEntity(this UpdateNotificationDto dto);

    public static partial NotificationTypeDto ToTypeDto(this NotificationType type);

    public static partial NotificationType ToType(this NotificationTypeDto typeDto);
}