namespace snowcoreBlog.Backend.Core.Interfaces.Steps;

/// <summary>
/// Implemented by pipeline contexts participating in durable step checkpointing. Snapshots are
/// typed (AOT-safe): each concrete snapshot type must be registered in the owning service's
/// checkpoint <c>JsonSerializerContext</c> so the coordinator can resolve <c>JsonTypeInfo</c>.
/// </summary>
public interface IStepCheckpointState
{
    /// <summary>Captures the checkpointable state as a typed snapshot (boxed).</summary>
    object CaptureCheckpoint();

    /// <summary>Restores state from a previously captured snapshot, mutating this instance in place.</summary>
    void RestoreCheckpoint(object snapshot);
}
