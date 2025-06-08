namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record AdminExistsValidationResult
{
    public bool Exists { get; set; } = false;
}