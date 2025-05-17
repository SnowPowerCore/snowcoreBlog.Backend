using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push.Core.Contracts;

/// <summary>
/// HTTP methods (verbs) to use with <see cref="NotificationHttpMethod"/>.
/// </summary>
public enum NotificationHttpMethod
{
    /// <summary>
    /// Represents the HTTP GET method.
    /// </summary>
    [JsonStringEnumMemberName("GET")]
    Get,

    /// <summary>
    /// Represents the HTTP POST method.
    /// </summary>
    [JsonStringEnumMemberName("POST")]
    Post,

    /// <summary>
    /// Represents the HTTP PUT method.
    /// </summary>
    [JsonStringEnumMemberName("PUT")]
    Put,

    /// <summary>
    /// Represents the HTTP DELETE method.
    /// </summary>
    [JsonStringEnumMemberName("DELETE")]
    Delete,

    /// <summary>
    /// Represents the HTTP PATCH method.
    /// </summary>
    [JsonStringEnumMemberName("PATCH")]
    Patch,

    /// <summary>
    /// Represents the HTTP OPTIONS method.
    /// </summary>
    [JsonStringEnumMemberName("OPTIONS")]
    Options
}