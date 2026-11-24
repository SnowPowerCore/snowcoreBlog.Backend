namespace snowcoreBlog.Backend.Documents.Options;

public record DocumentsUploadLimitsOptions
{
    /// <summary>Maximum accepted upload size in bytes. Default: 20 MiB.</summary>
    public long MaxSizeBytes { get; init; } = 20 * 1024 * 1024;

    /// <summary>MIME content types accepted by the upload pipeline. Empty = allow all.</summary>
    public string[] AllowedContentTypes { get; init; } = [];
}
