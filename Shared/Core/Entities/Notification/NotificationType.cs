namespace snowcoreBlog.Backend.Core.Entities.Notification;

/// <summary>
/// Type of technical notification for styling and categorization purposes.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// General informational notification.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning notification (e.g., scheduled maintenance).
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Success/positive notification.
    /// </summary>
    Success = 2,

    /// <summary>
    /// Important announcement about the website/service.
    /// </summary>
    Announcement = 3,

    /// <summary>
    /// Scheduled maintenance notification.
    /// </summary>
    Maintenance = 4,

    /// <summary>
    /// Outage or incident notification.
    /// </summary>
    Outage = 5,

    /// <summary>
    /// New feature or update to the website.
    /// </summary>
    FeatureUpdate = 6
}
