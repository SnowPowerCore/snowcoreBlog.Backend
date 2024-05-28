namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public record UserCreationResult
{
    public required Guid Id { get; init; }
}