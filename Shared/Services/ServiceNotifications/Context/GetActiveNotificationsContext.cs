using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.ServiceNotifications.Context;

public class GetActiveNotificationsContext(int? maxCount = null) : BaseGenericContext
{
    public int? MaxCount { get; } = maxCount;
}