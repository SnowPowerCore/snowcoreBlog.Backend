namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public record CreateUser
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public required string Email { get; init; } = string.Empty;

    public required string Password { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public bool Subscribed { get; init; } = false;

    public required bool ConfirmedAgreement { get; init; } = false;
}