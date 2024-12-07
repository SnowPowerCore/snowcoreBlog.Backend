namespace snowcoreBlog.Backend.Email.Core.Options;

public class SendGridSenderAccountOptions
{
    public required string SenderAddress { get; init; }

    public string? SenderName { get; init; } = string.Empty;
}