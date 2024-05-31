namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record AdminCreationResult
{
    public required Guid Id { get; init; }
}