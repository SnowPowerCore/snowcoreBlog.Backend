namespace snowcoreBlog.Backend.ApiAccessRestrictions.Entities;

public class ApiAccessResponseTemplateEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

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