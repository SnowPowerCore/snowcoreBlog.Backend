namespace snowcoreBlog.Backend.IAM.Core.Contracts;

/// <summary>
/// Generates (or regenerates) a TOTP shared secret for a pending temp user so
/// an authenticator app (Microsoft / Google Authenticator) can be enrolled
/// during registration.
/// </summary>
public sealed record SetupUserAuthenticator
{
    public required string Email { get; init; }

    public required string TempUserVerificationToken { get; init; }
}
