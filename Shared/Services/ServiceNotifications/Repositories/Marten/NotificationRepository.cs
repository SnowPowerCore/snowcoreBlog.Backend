using Marten;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.ServiceNotifications.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.ServiceNotifications.Repositories.Marten;

public class NotificationRepository(IDocumentSession session) : BaseMartenRepository<NotificationEntity>(session), INotificationRepository
{
    private readonly IDocumentSession _session = session;

    public async Task<IEnumerable<NotificationEntity>> GetActiveNotificationsAsync(int? maxCount = null, CancellationToken token = default)
    {
        var now = DateTime.UtcNow;

        var baseQuery = _session.Query<NotificationEntity>()
            .Where(n => n.IsActive)
            .Where(n => !n.StartDate.HasValue || n.StartDate <= now)
            .Where(n => !n.EndDate.HasValue || n.EndDate >= now)
            .OrderByDescending(n => n.Priority)
            .ThenByDescending(n => n.CreatedAt);

        if (maxCount.HasValue)
        {
            return await baseQuery.Take(maxCount.Value).ToListAsync(token);
        }

        return await baseQuery.ToListAsync(token);
    }
}