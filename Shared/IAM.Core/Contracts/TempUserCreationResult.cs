namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record TempUserCreationResult
{
    public required Guid Id { get; init; }

    public required string FirstName { get; init; }

    public required string Email { get; init; }

    public required bool InitialEmailConsent { get; init; }

    public required string VerificationToken { get; init; }

    public required DateTime VerificationTokenExpirationDate { get; init; }
}