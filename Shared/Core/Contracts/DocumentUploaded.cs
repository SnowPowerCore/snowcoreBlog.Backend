namespace snowcoreBlog.Backend.Core.Contracts;

/// <summary>
/// Published on RabbitMQ after a document was stored and its processing sequence
/// finished successfully. Consumed by any service that needs to react to new content
/// (e.g. thumbnailing, indexing, moderation) without coupling to the upload request.
/// </summary>
public sealed record DocumentUploaded(
    Guid DocumentId,
    string FileName,
    string ContentType,
    string Category,
    string Checksum,
    long SizeBytes,
    Guid? OwnerId);
