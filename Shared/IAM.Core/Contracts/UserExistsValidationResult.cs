namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserExistsValidationResult
{
    public bool Exists { get; init; } = false;

    /// <summary>
    /// Resolved unique identifier of the existing account.
    /// Populated only when <see cref="Exists"/> is true.
    /// </summary>
    public Guid? UserId { get; init; } = null;
}