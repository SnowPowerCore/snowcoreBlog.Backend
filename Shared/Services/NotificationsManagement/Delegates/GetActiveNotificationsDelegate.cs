using MaybeResults;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Delegates;

public delegate Task<IMaybe<List<NotificationDto>>> GetActiveNotificationsDelegate(GetActiveNotificationsContext context, CancellationToken token = default);