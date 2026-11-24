using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;

public record RegionRestrictionEntity : BaseEntity
{
    public string RegionCode { get; set; } = string.Empty; // e.g. ISO country code

    public RestrictionLevel Level { get; set; } = RestrictionLevel.Block;

    public List<string> AffectedPaths { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? ExpiresAt { get; set; }
}