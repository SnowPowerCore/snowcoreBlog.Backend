namespace snowcoreBlog.Backend.Email.Core.Options;

public record SendGridSenderAccountOptions
{
    public string SenderAddress { get; set; }

    public string? SenderName { get; set; } = string.Empty;
}