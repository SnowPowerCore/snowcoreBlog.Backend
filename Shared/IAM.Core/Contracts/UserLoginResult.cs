namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserLoginResult
{
    public required Guid Id { get; init; }
}