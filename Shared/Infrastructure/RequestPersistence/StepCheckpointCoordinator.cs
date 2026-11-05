using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.Core.Interfaces.Steps;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Bridges the AspectInjector-woven step advices (<see cref="StepCheckpointAspect"/>) to the
/// durable claim store.
/// <para>
/// Registered as a singleton; on construction it publishes itself to
/// <see cref="Current"/> — the static reference the woven advice reads (no package-supplied
/// service-locator involved). Per-operation dependencies are resolved through
/// <see cref="IServiceScopeFactory"/> so checkpoint writes never share the business session.
/// Eager construction is guaranteed by the recovery worker's constructor dependency.
/// </para>
/// <para>
/// <see cref="BeginStep"/> never performs I/O: resume state arrives via
/// <see cref="RequestResumeScope.Current"/> (set by the pre-processor). Context hydration mutates
/// the context instance in place, so the advice can proceed with the original arguments.
/// </para>
/// <para>
/// <see cref="TrackCompletion"/> captures the context at the moment the step's task completes
/// (continuation runs synchronously on the completing thread, before outer post-`next` logic) and
/// persists asynchronously. Deep-most steps complete first, so checkpoints materialize in reverse
/// pipeline order and resume consumes the highest contiguous index.
/// </para>
/// </summary>
public sealed class StepCheckpointCoordinator : IStepCheckpointCoordinator
{
    /// <summary>Static reference the woven advice reads; set when DI constructs the singleton.</summary>
    internal static StepCheckpointCoordinator? Current { get; private set; }

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JsonSerializerOptions _snapshotOptions;
    private readonly ILogger<StepCheckpointCoordinator> _logger;

    private const string WildcardPipelineKey = "*";

    public StepCheckpointCoordinator(
        IServiceScopeFactory scopeFactory,
        IJsonTypeInfoResolver snapshotSerializerResolver,
        ILogger<StepCheckpointCoordinator> logger)
    {
        _scopeFactory = scopeFactory;
        _snapshotOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = snapshotSerializerResolver
        };
        _logger = logger;
        Current = this;
    }

    public StepCheckpointDecision BeginStep(object step, object context)
    {
        var scope = RequestResumeScope.Current;
        if (scope is default(RequestResumeScope))
        {
            return StepCheckpointDecision.Run;
        }

        var stepTypeName = step.GetType().FullName ?? step.GetType().Name;
        var pipelineKey = context.GetType().Name;

        if (!string.Equals(scope.PipelineKey, pipelineKey, StringComparison.Ordinal)
            && !string.Equals(scope.PipelineKey, WildcardPipelineKey, StringComparison.Ordinal))
        {
            // Different pipeline running inside a persisted request scope (unexpected) — do not interfere.
            return StepCheckpointDecision.Run;
        }

        if (!scope.StepPositions.TryGetValue(stepTypeName, out var position))
        {
            position = scope.StepPositions.Count;
            scope.StepPositions[stepTypeName] = position;
        }

        // Hydrate exactly once per resumed execution — every subsequent skip/run observes the
        // restored context state.
        if (!scope.RestoreApplied)
        {
            scope.RestoreApplied = true;
            if (scope.RestoreState is not default(ResumeStepState))
            {
                HydrateContext(context, pipelineKey, scope.RestoreState);
            }
            else if (step.GetType().IsDefined(typeof(CheckpointedStepAttribute), inherit: false)
                     && context is not IStepCheckpointState)
            {
                // Fail fast: pipeline opted into checkpointing (attributes present on the very
                // first executed step) but its context cannot produce snapshots.
                if (scope.StepPositions.Count == 1 && position == 0)
                {
                    throw new InvalidOperationException(
                        $"Pipeline context '{pipelineKey}' does not implement IStepCheckpointState — "
                        + "checkpointed steps require snapshot-capable contexts.");
                }
            }
        }

        return scope.CompletedStepTypes.Contains(stepTypeName)
            ? StepCheckpointDecision.Skip
            : StepCheckpointDecision.Run;
    }

    public void TrackCompletion(object step, Task stepTask, object context)
    {
        var scope = RequestResumeScope.Current;
        if (scope is default(RequestResumeScope) || stepTask is default(Task))
        {
            return;
        }

        var stepTypeName = step.GetType().FullName ?? step.GetType().Name;
        if (!scope.StepPositions.TryGetValue(stepTypeName, out var position)
            || context is not IStepCheckpointState)
        {
            return;
        }

        _ = stepTask.ContinueWith(
            _ => PersistCheckpointAsync(scope, position, stepTypeName, context),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    private void HydrateContext(object context, string pipelineKey, ResumeStepState restoreState)
    {
        if (context is not IStepCheckpointState state)
        {
            throw new InvalidOperationException(
                $"Pipeline context '{pipelineKey}' does not implement IStepCheckpointState — cannot "
                + $"resume from checkpoint (snapshot '{restoreState.SnapshotType}'). Register snapshot "
                + "providers for checkpointed pipelines.");
        }

        var snapshotType = Type.GetType(restoreState.SnapshotType, throwOnError: false);
        if (snapshotType is default(Type))
        {
            throw new InvalidOperationException(
                $"Checkpoint snapshot type '{restoreState.SnapshotType}' could not be resolved. Did the "
                + "snapshot type move or get renamed since the request was persisted?");
        }

        var snapshot = JsonSerializer.Deserialize(restoreState.SnapshotJson, _snapshotOptions.GetTypeInfo(snapshotType))
                       ?? throw new InvalidOperationException(
                           $"Checkpoint snapshot '{restoreState.SnapshotType}' deserialized to null.");

        state.RestoreCheckpoint(snapshot);
    }

    private async Task PersistCheckpointAsync(
        RequestResumeScope scope,
        int position,
        string stepTypeName,
        object context)
    {
        try
        {
            var snapshot = ((IStepCheckpointState)context).CaptureCheckpoint();
            var snapshotType = snapshot.GetType();
            var json = JsonSerializer.Serialize(snapshot, _snapshotOptions.GetTypeInfo(snapshotType));

            await using var persistScope = _scopeFactory.CreateAsyncScope();
            var repository = persistScope.ServiceProvider.GetRequiredService<IRequestPersistenceRepository>();
            await repository.AppendCheckpointAsync(new RequestCheckpointEntity
            {
                Id = Guid.CreateVersion7(),
                PersistedRequestId = scope.PersistedRequestId,
                PipelineKey = context.GetType().Name,
                StepIndex = position,
                StepTypeName = stepTypeName,
                SnapshotType = snapshotType.AssemblyQualifiedName ?? snapshotType.FullName ?? snapshotType.Name,
                SnapshotJson = json,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to persist checkpoint (request {RequestId}, step {Step}) — replay will restart from the previous checkpoint.",
                scope.PersistedRequestId,
                stepTypeName);
        }
    }
}
