using snowcoreBlog.Backend.Core.Entities.Documents;

namespace snowcoreBlog.Backend.Documents.Context;

/// <summary>
/// Carrier for the stepified upload pipeline. Steps consume the request part and
/// accumulate their outputs on the mutable tail, so any step can be re-ordered,
/// replaced or extended (e.g. a virus-scan or OCR step in a domain-specific service)
/// without changing the delegate signature.
/// </summary>
public class UploadDocumentContext(
    Stream content,
    string fileName,
    string contentType,
    string? category,
    Guid? ownerId,
    Dictionary<string, string>? metadata)
{
    // Request (immutable part)
    public Stream Content { get; } = content;
    public string FileName { get; } = fileName;
    public string ContentType { get; } = contentType;
    public string? Category { get; } = category;
    public Guid? OwnerId { get; } = ownerId;
    public Dictionary<string, string>? Metadata { get; } = metadata;

    // Filled by steps
    public string? Checksum { get; set; }
    public string? ContentKey { get; set; }
    public bool IsDuplicate { get; set; }
    public DocumentEntity? Document { get; set; }
}
