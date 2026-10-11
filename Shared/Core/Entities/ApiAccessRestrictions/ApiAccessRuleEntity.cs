namespace snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

public class ApiAccessRuleEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Enabled { get; set; } = true;

    public int Priority { get; set; } = 0;

    public RestrictionLevel Action { get; set; } = RestrictionLevel.Block;

    public List<string> Methods { get; set; } = new();

    public List<string> PathPatterns { get; set; } = new();

    public List<string> Tags { get; set; } = new();

    public List<string> IpRanges { get; set; } = new();

    public List<string> RegionCodes { get; set; } = new();

    public Guid? ResponseTemplateId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; set; }
}