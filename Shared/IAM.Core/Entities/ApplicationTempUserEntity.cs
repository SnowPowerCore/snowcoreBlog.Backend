using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public record ApplicationTempUserEntity : BaseEntity
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    public required string ActivationToken { get; init; }

    public required DateTimeOffset ActivationTokenExpirationDate { get; init; }

    public string? PhoneNumber { get; init; }

    public required string FirstName { get; init; }

    public string? LastName { get; init; } = string.Empty;

    public required bool ConfirmedAgreement { get; init; }

    public bool InitialEmailConsent { get; init; } = true;

    /// <summary>
    /// Base32-encoded TOTP shared secret generated while the pending
    /// registration is being completed with an authenticator app.
    /// Moved onto the real user entity once the first code is verified.
    /// </summary>
    public string? TotpSecret { get; init; }
}