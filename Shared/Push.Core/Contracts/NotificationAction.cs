using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push.Core.Contracts;

public class NotificationAction
{
    /// <summary>
    /// Gets a value indicating whether the notification should be cleared.
    /// </summary>
    [JsonPropertyName("clear")]
    public bool ClearNotification { get; init; } = false;

    /// <summary>
    /// Gets the label of the action.
    /// </summary>
    [JsonPropertyName("label")]
    public required string Label { get; init; }

    /// <summary>
    /// Gets the type of the action.
    /// </summary>
    [JsonPropertyName("action")]
    public virtual string Type { get; init; } = string.Empty;
}