namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Decision computed for a checkpointed step entering execution.
/// </summary>
public sealed class StepCheckpointDecision
{
    /// <summary>Proceed into the step body (default).</summary>
    public static StepCheckpointDecision Run { get; } = new();

    /// <summary>Skip the step body and continue with the remaining pipeline (resume path).</summary>
    public static StepCheckpointDecision Skip { get; } = new() { SkipStep = true };

    private StepCheckpointDecision()
    {
    }

    /// <summary>When <c>true</c>, the aspect bypasses the step body and continues with the rest of the chain.</summary>
    public bool SkipStep { get; private init; }
}
