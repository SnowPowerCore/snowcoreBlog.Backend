namespace snowcoreBlog.Backend.AdminsManagement.Options;

public record AdminEmailSenderOptions
{
    public string SenderAddress { get; set; } = string.Empty;

    public string SenderName { get; set; } = "snowcore's Blog Admin";
}