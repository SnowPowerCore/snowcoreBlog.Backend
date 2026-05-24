using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.UrlShortener.Core.Entities;

public record UrlMappingEntity : BaseEntity
{
    public string Code { get; init; } = string.Empty;
    public string OriginalUrl { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public int? MaxClicksPerWindow { get; init; }
    public int? WindowDurationSeconds { get; init; }
    public bool IsActive { get; init; } = true;
}
