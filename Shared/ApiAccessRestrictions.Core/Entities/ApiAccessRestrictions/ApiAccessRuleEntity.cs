using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;

public record ApiAccessRuleEntity : BaseEntity
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Enabled { get; set; } = true;

    public int Priority { get; set; } = 0;

    public RestrictionLevel Action { get; set; } = RestrictionLevel.Block;

    public List<string> Methods { get; set; } = [];

    public List<string> PathPatterns { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public List<string> IpRanges { get; set; } = [];

    public List<string> RegionCodes { get; set; } = [];

    public Guid? ResponseTemplateId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; set; }
}