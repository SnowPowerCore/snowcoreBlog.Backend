using MaybeResults;
using snowcoreBlog.Backend.NotificationsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.NotificationsManagement.Delegates;

public delegate Task<IMaybe<CreateNotificationResultDto>> CreateNotificationDelegate(CreateNotificationContext context, CancellationToken token = default);
