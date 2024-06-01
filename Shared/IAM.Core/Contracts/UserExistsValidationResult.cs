namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserExistsValidationResult
{
    public bool Exists { get; init; } = false;
}