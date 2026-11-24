using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;

/// <summary>
/// A completed step boundary of an interrupted pipeline: the step index within the pipeline, the
/// step type that finished, and an AOT-safe serialized snapshot of the pipeline context as it was
/// when the step returned. Replay resumes from the highest contiguous checkpoint.
/// </summary>
public record RequestCheckpointEntity : BaseEntity
{
    public Guid PersistedRequestId { get; init; }

    /// <summary>Pipeline identity (context type name) вЂ” groups checkpoints belonging to one pipeline run.</summary>
    public string PipelineKey { get; init; } = string.Empty;

    /// <summary>Zero-based step position; deep-most steps complete first, so higher indexes imply deeper completion.</summary>
    public int StepIndex { get; init; }

    public string StepTypeName { get; init; } = string.Empty;

    /// <summary>Snapshot CLR type name (assembly-qualified short form) used to resolve JsonTypeInfo.</summary>
    public string SnapshotType { get; init; } = string.Empty;

    public string SnapshotJson { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
