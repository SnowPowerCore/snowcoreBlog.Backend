using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

public record IpRestrictionEntity : BaseEntity
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    // CIDR ranges, single IPs, or range notation
    public List<string> IpRanges { get; set; } = [];

    public bool IsBlocked { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? ExpiresAt { get; set; }
}