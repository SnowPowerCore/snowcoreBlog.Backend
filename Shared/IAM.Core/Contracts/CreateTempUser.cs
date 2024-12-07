namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record CreateTempUser
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    public string? PhoneNumber { get; set; }

    public required string FirstName { get; init; }

    public string? LastName { get; init; } = string.Empty;

    public required bool ConfirmedAgreement { get; init; }

    public bool InitialEmailConsent { get; init; } = true;
}