namespace snowcoreBlog.Backend.Push.Core.Contracts;

public sealed record SendGenericPush
{
    public required string Topic { get; init; }

    public required string Subject { get; init; }

    public string? Content { get; init; } = string.Empty;

    public bool IsContentMarkdown { get; init; } = false;

    public string? IconUri { get; init; } = string.Empty;

    public string? Email { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; } = string.Empty;

    public NotificationPriority Priority { get; init; } = NotificationPriority.Default;

    public NotificationAttachment? Attachment { get; init; }

    public List<NotificationAction> Actions { get; init; } = [];

    public string? ClickUri { get; init; } = string.Empty;

    public IReadOnlyList<string> Tags { get; init; } = [];
}