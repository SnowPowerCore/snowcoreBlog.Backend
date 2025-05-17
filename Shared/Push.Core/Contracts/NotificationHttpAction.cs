using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push.Core.Contracts;

public class NotificationHttpAction : NotificationAction
{
    /// <inheritdoc/>
    public override string Type => "http";

    /// <summary>
    /// Gets the URL for the HTTP action.
    /// </summary>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string Url { get; init; }

    /// <summary>
    /// Gets the body of the HTTP request.
    /// </summary>
    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Body { get; init; }

    /// <summary>
    /// Gets the headers of the HTTP request.
    /// </summary>
    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Headers { get; init; }

    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    [JsonPropertyName("method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NotificationHttpMethod? Method { get; init; }
}