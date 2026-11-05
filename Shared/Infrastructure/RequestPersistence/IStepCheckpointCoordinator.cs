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

/// <summary>
/// Contract consulted by the AspectInjector-woven step advices for steps marked with
/// <see cref="CheckpointedStepAttribute"/>. The application registers exactly one implementation;
/// when the woven advice observes none, it behaves as a pure pass-through.
/// <para>
/// Implementations must be effectively singleton-safe: all per-operation dependencies are resolved
/// internally (e.g. via <see cref="IServiceScopeFactory"/>) because the advice holds a static
/// reference. <see cref="BeginStep"/> must never block on I/O — resume state arrives through
/// <see cref="RequestResumeScope.Current"/> (established by the pre-processor).
/// </para>
/// </summary>
public interface IStepCheckpointCoordinator
{
    /// <summary>
    /// Called before the step body executes. On resume, implementations hydrate the context in
    /// place (mutate the instance) and instruct the advice to skip steps that already completed.
    /// </summary>
    /// <param name="step">The step instance entering execution.</param>
    /// <param name="context">The pipeline context instance (may be mutated in place for hydration).</param>
    /// <returns>Whether to run or skip the step body.</returns>
    StepCheckpointDecision BeginStep(object step, object context);

    /// <summary>
    /// Called right after the step invocation returned its task. Implementations capture the
    /// context when the task completes — deep-most steps complete first, so checkpoints
    /// materialize in reverse pipeline order — and persist them asynchronously.
    /// <para>Must never throw synchronously.</para>
    /// </summary>
    void TrackCompletion(object step, Task stepTask, object context);
}
