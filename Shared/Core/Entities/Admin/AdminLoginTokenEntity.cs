using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Admin;

/// <summary>
/// One-time admin login token (emailed magic link). Raw token lives only in
/// the link; the hash is stored here. Single-use, time-limited.
/// </summary>
public record AdminLoginTokenEntity : BaseEntity
{
    public required string Email { get; init; }

    public required string TokenHash { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public DateTime ExpiresAtUtc { get; init; }

    public DateTime? UsedAtUtc { get; init; }

    public bool IsUsable => UsedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;
}
