namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateTempUserExists
{
    public required string Email { get; init; } = string.Empty;
}