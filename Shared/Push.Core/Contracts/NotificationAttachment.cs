namespace snowcoreBlog.Backend.Push.Core.Contracts;

public class NotificationAttachment
{
    public required string Uri { get; init; }

    public string Name { get; init; } = string.Empty;
}
