namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateUserNickNameTaken
{
    public required string NickName { get; init; }
}