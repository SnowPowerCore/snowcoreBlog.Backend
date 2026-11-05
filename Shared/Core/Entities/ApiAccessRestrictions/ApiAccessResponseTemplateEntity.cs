using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

public record ApiAccessResponseTemplateEntity : BaseEntity
{
    public string? Name { get; set; }

    public int StatusCode { get; set; } = 403;

    public string ContentType { get; set; } = "application/json";

    /// <summary>
    /// JSON string to write directly to the HTTP response body when blocked.
    /// </summary>
    public string? BodyJson { get; set; }

    public string? Reason { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}