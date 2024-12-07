namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record TempUserExistsValidationResult
{
    public bool Exists { get; init; } = false;
}