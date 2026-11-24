namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Ambient (async-local) resume state for a persisted request, established by the pre-processor and
/// consumed by <c>StepCheckpointCoordinator</c> through the woven step advices. A scope with no
/// checkpoints means "run from scratch but record checkpoints".
/// </summary>
public sealed class RequestResumeScope
{
    private static readonly AsyncLocal<RequestResumeScope?> _current = new();

    public static RequestResumeScope? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    public required Guid PersistedRequestId { get; init; }

    /// <summary>
    /// Pipeline identity (context type name). <c>"*"</c> for from-scratch runs where the pipeline
    /// is not yet known — the coordinator binds to whatever pipeline starts.
    /// </summary>
    public required string PipelineKey { get; init; }

    /// <summary>Snapshot of the highest contiguous completed step; hydration source. <c>null</c> = run from scratch.</summary>
    public ResumeStepState? RestoreState { get; init; }

    /// <summary>Fully-qualified type names of completed steps (skip decisions on resume).</summary>
    public required IReadOnlySet<string> CompletedStepTypes { get; init; }

    /// <summary>Coordinator-internal: step position registry (execution order == pipeline position).</summary>
    internal Dictionary<string, int> StepPositions { get; } = new(StringComparer.Ordinal);

    internal bool RestoreApplied { get; set; }
}
