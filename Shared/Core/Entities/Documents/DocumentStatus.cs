namespace snowcoreBlog.Backend.Core.Entities.Documents;

/// <summary>
/// Lifecycle status of a stored document. Mirrored for the public API by
/// <c>snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentStatusDto</c>.
/// </summary>
public enum DocumentStatus
{
    /// <summary>Content bytes were accepted and persisted, but no further processing ran yet.</summary>
    Uploaded = 0,

    /// <summary>An (extensible) processing pipeline is currently working on the document.</summary>
    Processing = 1,

    /// <summary>The whole processing sequence finished successfully.</summary>
    Processed = 2,

    /// <summary>At least one processing step rejected the document.</summary>
    Rejected = 3
}
