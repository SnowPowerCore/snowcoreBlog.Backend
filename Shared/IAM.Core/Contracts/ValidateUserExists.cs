namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateUserExists
{
    public required string Email { get; init; } = string.Empty;
}