using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Admin;

/// <summary>
/// An administrator account. Every admin except the very first one MUST
/// reference the admin who invited them (<see cref="InvitedByAdminId"/>) —
/// enforced on the database level by a Postgres FK (nullable column, the
/// null is reserved for the bootstrap admin only).
/// </summary>
public record AdminEntity : BaseEntity
{
    public required string Email { get; init; }

    public string NickName { get; init; } = string.Empty;

    /// <summary>The admin who invited this admin. NULL only for the bootstrap admin.</summary>
    public Guid? InvitedByAdminId { get; init; }

    /// <summary>IAM identity (ApplicationAdminEntity.Id) created through the IAM CreateAdmin request.</summary>
    public required Guid IamUserId { get; init; }

    public DateTime InvitedAtUtc { get; init; } = DateTime.UtcNow;

    public bool IsActive { get; init; } = true;
}
