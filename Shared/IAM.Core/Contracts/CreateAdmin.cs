namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record CreateAdmin
{
    public required Guid FromAdmin { get; init; } = Guid.Empty;

    public string NickName { get; init; } = string.Empty;

    public required string Email { get; init; } = string.Empty;
}