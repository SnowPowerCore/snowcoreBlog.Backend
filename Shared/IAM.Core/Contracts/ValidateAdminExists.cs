namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateAdminExists
{
    public required string Email { get; init; } = string.Empty;
}