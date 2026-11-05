using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Admin;

/// <summary>
/// A pending admin invitation created by an existing admin. The invitee
/// proceeds via an emailed link carrying the (hash-stored) token.
/// </summary>
public record AdminInviteEntity : BaseEntity
{
    public required string Email { get; init; }

    public string NickName { get; init; } = string.Empty;

    /// <summary>SHA-256 hex of the invite token (raw token only lives in the emailed link).</summary>
    public required string TokenHash { get; init; }

    /// <summary>The inviting admin. NOT nullable — invites can only be created by admins.</summary>
    public required Guid InvitedByAdminId { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public DateTime ExpiresAtUtc { get; init; }

    public DateTime? AcceptedAtUtc { get; init; }

    public bool IsAccepted => AcceptedAtUtc is not null;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsUsable => !IsAccepted && !IsExpired;
}
