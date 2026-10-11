using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.UrlShortener;

public record ClickEventEntity : BaseEntity
{
    public Guid UrlMappingId { get; init; }
    public DateTime ClickedAt { get; init; }
    public string? RemoteIp { get; init; }
}
