namespace snowcoreBlog.Backend.IAM.Core.Contracts;

/// <summary>
/// Completes a registration by verifying the first TOTP code from the
/// authenticator app instead of creating a passkey credential.
/// </summary>
public sealed record CreateUserWithAuthenticator
{
    public required string Email { get; init; }

    public required string TempUserVerificationToken { get; init; }

    public required string Code { get; init; }
}
