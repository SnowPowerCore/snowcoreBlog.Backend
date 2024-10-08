namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserTokenGenerationResult
{
    public required string Token { get; init; }
}