namespace snowcoreBlog.Backend.Push.Core.Contracts;

public sealed record SendGenericPush
{
    public required string SenderAddress { get; init; }

    public string? SenderName { get; init; } = string.Empty;

    public string NotifiedEntityAddress { get; init; } = string.Empty;

    public string? NotifiedEntityName { get; init; } = string.Empty;

    public required string Subject { get; init; }

    public string? Content { get; init; } = string.Empty;
}