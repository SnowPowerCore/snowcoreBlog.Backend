namespace snowcoreBlog.Backend.Infrastructure;

public sealed record ReaderRefreshTokenRecord
{
    public required Guid UserId { get; init; }

    public required Dictionary<string, string> Claims { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
