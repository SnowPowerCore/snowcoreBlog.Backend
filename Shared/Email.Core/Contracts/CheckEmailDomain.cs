namespace snowcoreBlog.Backend.Email.Core.Contracts;

public sealed record CheckEmailDomain
{
    public required string Email { get; init; }
}