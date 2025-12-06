using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Notification;

/// <summary>
/// Represents a top bar technical notification for website visitors (maintenance, outages, announcements, etc.).
/// </summary>
public record NotificationEntity : BaseEntity
{
    /// <summary>
    /// The title/text displayed in the notification bar.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Optional description or additional message with more details.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Optional URL for more information (e.g., status page, changelog).
    /// </summary>
    public string? LinkUrl { get; init; }

    /// <summary>
    /// Optional text for the link button/anchor.
    /// </summary>
    public string? LinkText { get; init; }

    /// <summary>
    /// Type of notification for styling purposes (e.g., Info, Warning, Maintenance, Outage).
    /// </summary>
    public required NotificationType Type { get; init; }

    /// <summary>
    /// Priority for ordering notifications (higher = more important).
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Whether this notification is currently active and should be displayed.
    /// </summary>
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// Whether this notification can be dismissed by the user.
    /// </summary>
    public bool IsDismissible { get; init; } = true;

    /// <summary>
    /// When this notification becomes active.
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// When this notification expires and should no longer be shown.
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// When this notification was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// When this notification was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; init; }

    /// <summary>
    /// User ID of who created this notification (admin).
    /// </summary>
    public Guid? CreatedByUserId { get; init; }
}
