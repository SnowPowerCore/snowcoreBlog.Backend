using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;

public interface INotificationRepository : IRepository<NotificationEntity>
{
    /// <summary>
    /// Get all active notifications that are currently within their valid date range.
    /// </summary>
    Task<IEnumerable<NotificationEntity>> GetActiveNotificationsAsync(int? maxCount = null, CancellationToken token = default);
}