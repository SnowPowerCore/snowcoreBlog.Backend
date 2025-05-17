using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push.Core.Contracts;

public class NotificationBroadcastAction : NotificationAction
{
    /// <inheritdoc/>
    public override string Type => "broadcast";

    /// <summary>
    /// Gets the extras for the broadcast action.
    /// </summary>
    [JsonPropertyName("extras")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Extras { get; init; }

    /// <summary>
    /// Gets the intent for the broadcast action.
    /// </summary>
    [JsonPropertyName("intent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Intent { get; init; }

    /// <summary>
    /// Gets the URL for the broadcast action.
    /// </summary>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }
}