using MaybeResults;
using snowcoreBlog.Backend.ServiceNotifications.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ServiceNotifications.Delegates;

public delegate Task<IMaybe<DeleteNotificationResultDto>> DeleteNotificationDelegate(DeleteNotificationContext context, CancellationToken token = default);