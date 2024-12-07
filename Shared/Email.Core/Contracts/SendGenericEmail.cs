namespace snowcoreBlog.Backend.Email.Core.Contracts;

public sealed record SendGenericEmail
{
    public required string SenderAddress { get; init; }

    public string? SenderName { get; init; } = string.Empty;

    public required string ReceiverAddress { get; init; } = string.Empty;

    public string? ReceiverName { get; init; } = string.Empty;

    public required string Subject { get; init; }

    public string PreHeader { get; init; } = string.Empty;

    public string? Content { get; init; } = string.Empty;
}