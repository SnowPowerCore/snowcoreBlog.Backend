namespace snowcoreBlog.Backend.Push.Core.Contracts;

/// <summary>
/// Represents priority levels.
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Represents the minimum priority level.
    /// </summary>
    Min = 1,

    /// <summary>
    /// Represents a low priority level.
    /// </summary>
    Low = 2,

    /// <summary>
    /// Represents the default priority level.
    /// </summary>
    Default = 3,

    /// <summary>
    /// Represents a high priority level.
    /// </summary>
    High = 4,

    /// <summary>
    /// Represents the maximum priority level.
    /// </summary>
    Max = 5
}