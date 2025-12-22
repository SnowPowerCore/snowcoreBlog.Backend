using MaybeResults;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Delegates;

public delegate Task<IMaybe<DeleteNotificationResultDto>> DeleteNotificationDelegate(DeleteNotificationContext context, CancellationToken token = default);