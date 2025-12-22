using MaybeResults;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Delegates;

public delegate Task<IMaybe<List<NotificationDto>>> GetActiveNotificationsDelegate(GetActiveNotificationsContext context, CancellationToken token = default);