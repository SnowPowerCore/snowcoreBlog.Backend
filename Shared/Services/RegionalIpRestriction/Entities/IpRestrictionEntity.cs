using System;
using System.Collections.Generic;
using Marten;
using Marten.Schema;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Entities;

public class IpRestrictionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Description { get; set; }
    // CIDR ranges, single IPs, or range notation
    public List<string> IpRanges { get; set; } = new();
    public bool IsBlocked { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; set; }
}
