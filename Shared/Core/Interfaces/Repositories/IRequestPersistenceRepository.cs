using snowcoreBlog.Backend.Core.Entities.RequestPersistence;

namespace snowcoreBlog.Backend.Core.Interfaces.Repositories;

public interface IRequestPersistenceRepository
{
    /// <summary>Durably inserts a new claim (own transaction, independent of the business session).</summary>
    Task<PersistedRequestEntity> CreateAsync(PersistedRequestEntity entity, CancellationToken ct = default);

    Task<PersistedRequestEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PersistedRequestEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default);

    /// <summary>Persists the full entity state (status transitions, lease, response).</summary>
    Task<PersistedRequestEntity> UpdateAsync(PersistedRequestEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Atomic takeover: transitions into Processing with a fresh lease (ownership token) only when
    /// the row is eligible — Processing with an expired lease (<c>LeaseUntil &lt;= now</c>),
    /// Persisted (never executed), or due RetryNeeded. RetryCount is bumped on recovered
    /// takeovers. The win is verified by re-reading OUR lease token from the row; returns
    /// <c>false</c> when another instance owns the lease.
    /// </summary>
    Task<bool> TryTakeOverAsync(Guid id, int expectedRetryCount, DateTimeOffset now, TimeSpan leaseTtl, CancellationToken ct = default);

    /// <summary>Terminal failure with an operator-facing note (e.g. replay re-auth failed).</summary>
    Task MarkReplayFailureAsync(Guid id, string note, CancellationToken ct = default);

    /// <summary>Durably appends one completed-step checkpoint (own transaction, Await semantics).</summary>
    Task AppendCheckpointAsync(RequestCheckpointEntity checkpoint, CancellationToken ct = default);

    /// <summary>Checkpoints of a request ordered by step index.</summary>
    Task<IReadOnlyList<RequestCheckpointEntity>> GetCheckpointsAsync(Guid persistedRequestId, CancellationToken ct = default);

    /// <summary>
    /// Rows eligible for recovery: aged Persisted (claimed but never executed), due RetryNeeded,
    /// Processing with expired lease. Limited batch per poll.
    /// </summary>
    Task<IReadOnlyList<PersistedRequestEntity>> GetRecoveryCandidatesAsync(DateTimeOffset now, TimeSpan recoverPersistedAfter, int limit, CancellationToken ct = default);

    Task DeleteCheckpointsAsync(Guid persistedRequestId, CancellationToken ct = default);

    /// <summary>Purges payloads/checkpoint linkage and marks the row as a dedup tombstone.</summary>
    Task TombstoneAsync(Guid id, DateTimeOffset completedAt, CancellationToken ct = default);
}
