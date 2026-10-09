namespace snowcoreBlog.Backend.IAM.Core.Contracts;

/// <summary>
/// Verifies a 6-digit code from the account's enrolled authenticator app as an
/// alternative to a passkey assertion during sign-in.
/// </summary>
public sealed record ValidateUserOneTimeCode
{
    public required string Email { get; init; }

    public required string Code { get; init; }
}
