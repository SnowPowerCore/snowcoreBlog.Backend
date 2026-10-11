using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Documents;

/// <summary>
/// Metadata record of a stored document. The binary content itself never lives in
/// Postgres — it is addressed by <see cref="ContentKey"/> inside the configured
/// <c>IDocumentContentStore</c> (file system store by default, swappable for blob storage).
/// </summary>
public record DocumentEntity : BaseEntity
{
    /// <summary>Original file name as uploaded by the client (no path segments).</summary>
    public required string FileName { get; init; }

    /// <summary>MIME content type reported at upload time.</summary>
    public required string ContentType { get; init; }

    /// <summary>Exact content length in bytes.</summary>
    public long SizeBytes { get; init; }

    /// <summary>
    /// Free-form grouping key used by consuming services to partition documents
    /// (e.g. <c>general</c>, <c>avatar</c>, <c>legal</c>). Domain-specific services
    /// (a future legal-documents microservice) define their own categories on top.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>Lifecycle status of the document (see <see cref="DocumentStatus"/>).</summary>
    public DocumentStatus Status { get; init; } = DocumentStatus.Uploaded;

    /// <summary>Hex-encoded SHA-256 hash of the content; the deduplication identity.</summary>
    public required string Checksum { get; init; }

    /// <summary>
    /// Content-addressable key inside the <c>IDocumentContentStore</c>. For the
    /// file-system store this equals the checksum, which makes storage deduplicated.
    /// </summary>
    public required string ContentKey { get; init; }

    /// <summary>Optional owner (user/author id) the document belongs to.</summary>
    public Guid? OwnerId { get; init; }

    /// <summary>Optional free-form metadata exchanged by processing steps or consumers.</summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>UTC timestamp of the upload.</summary>
    public DateTime UploadedAt { get; init; }

    /// <summary>UTC timestamp when the processing sequence finished (null while pending).</summary>
    public DateTime? ProcessedAt { get; init; }
}
