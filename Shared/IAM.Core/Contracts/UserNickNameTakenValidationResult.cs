namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserNickNameTakenValidationResult
{
    public bool WasTaken { get; init; } = false;
}