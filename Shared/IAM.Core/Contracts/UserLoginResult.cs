namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserLoginResult
{
    public required Guid Id { get; init; }

    /// <summary>Email of the authenticated user as resolved by IAM — may differ from the email entered in the login form.</summary>
    public required string Email { get; init; }
}