using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push.Core.Contracts;

public class NotificationViewAction : NotificationAction
{
    /// <inheritdoc/>
    public override string Type => "view";

    /// <summary>
    /// Gets the URL for the view action.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}