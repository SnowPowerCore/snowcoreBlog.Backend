namespace snowcoreBlog.Backend.Documents.Options;

public record DocumentsStorageOptions
{
    /// <summary>
    /// Root folder of the default file-system content store. Relative paths are
    /// resolved against the application base directory. Swap the registered
    /// <c>IDocumentContentStore</c> implementation to move content to blob storage.
    /// </summary>
    public string? RootPath { get; init; }
}

public record DocumentsUploadLimitsOptions
{
    /// <summary>Maximum accepted upload size in bytes. Default: 20 MiB.</summary>
    public long MaxSizeBytes { get; init; } = 20 * 1024 * 1024;

    /// <summary>MIME content types accepted by the upload pipeline. Empty = allow all.</summary>
    public string[] AllowedContentTypes { get; init; } = [];
}

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
