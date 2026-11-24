namespace snowcoreBlog.Backend.Documents.Options;

public record DocumentsOptions
{
    /// <summary>
    /// Public (gateway) base URL of this service, used to build absolute
    /// download URLs returned to clients.
    /// </summary>
    public string PublicBaseUrl { get; init; } = "https://localhost/api/documents";

    public DocumentsStorageOptions Storage { get; init; } = new();

    public DocumentsUploadLimitsOptions UploadLimits { get; init; } = new();
}
