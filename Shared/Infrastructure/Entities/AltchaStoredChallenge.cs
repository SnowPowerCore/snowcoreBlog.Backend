using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Infrastructure.Entities;

public record AltchaStoredChallenge : BaseEntity
{
    public required string Challenge { get; set; }
    
    public required DateTimeOffset ExpiryUtc { get; set; }
}