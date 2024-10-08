namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record GenerateUserToken
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public string[] Roles { get; init; } = ["User"];
}