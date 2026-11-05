namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record DeleteUser
{
    public required Guid UserId { get; init; }
}
