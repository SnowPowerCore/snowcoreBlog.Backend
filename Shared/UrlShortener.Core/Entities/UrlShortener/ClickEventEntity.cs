using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener;

public record ClickEventEntity : BaseEntity
{
    public Guid UrlMappingId { get; init; }
    public DateTime ClickedAt { get; init; }
    public string? RemoteIp { get; init; }
}
