using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.Infrastructure.Entities;

public record AltchaStoredChallengeEntity : BaseEntity
{
    public required string Challenge { get; set; }
    
    public required DateTimeOffset ExpiryUtc { get; set; }
}