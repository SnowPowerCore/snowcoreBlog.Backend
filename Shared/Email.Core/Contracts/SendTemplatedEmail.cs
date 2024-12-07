namespace snowcoreBlog.Backend.Email.Core.Contracts;

public sealed record SendTemplatedEmail
{
    public required string SenderAddress { get; init; }

    public string? SenderName { get; init; } = string.Empty;

    public required string ReceiverAddress { get; init; } = string.Empty;

    public string? ReceiverName { get; init; } = string.Empty;

    public required string TemplateId { get; init; }

    public object DynamicTemplateData { get; init; } = new();
}