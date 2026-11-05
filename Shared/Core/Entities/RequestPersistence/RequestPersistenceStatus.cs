namespace snowcoreBlog.Backend.Core.Entities.RequestPersistence;

public enum RequestPersistenceStatus
{
    /// <summary>Reserved for future pre-claim intake telemetry; not used by the current flow.</summary>
    Received = 0,

    /// <summary>Durable claim written; execution not yet started (replayable from scratch).</summary>
    Persisted = 1,

    /// <summary>Execution in progress; lease held by the executing instance.</summary>
    Processing = 2,

    /// <summary>Terminal. Response captured (non-streaming) and completion marker durable.</summary>
    Succeeded = 3,

    /// <summary>Terminal. Deterministic failure or exhausted retry budget; manual handling.</summary>
    Failed = 4,

    /// <summary>Transient failure; recovery worker replays after backoff.</summary>
    RetryNeeded = 5
}
