namespace snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

public class RegionRestrictionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string RegionCode { get; set; } = string.Empty; // e.g. ISO country code

    public RestrictionLevel Level { get; set; } = RestrictionLevel.Block;

    public List<string> AffectedPaths { get; set; } = new();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? ExpiresAt { get; set; }
}