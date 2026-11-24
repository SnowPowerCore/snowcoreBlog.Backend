using Marten;
using Marten.Patching;
using snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Infrastructure.Repositories;

/// <summary>
/// Marten-backed claim store. Every operation opens its own short-lived session вЂ” durability of
/// claims/checkpoints is never entangled with the business session state of the running request.
/// Takeover is an atomic conditional update (patched statements + increment committed in one
/// transaction), verified by a fresh re-read.
/// </summary>
public class RequestPersistenceRepository(IDocumentStore store) : IRequestPersistenceRepository
{
    public async Task<PersistedRequestEntity> CreateAsync(PersistedRequestEntity entity, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        session.Insert(entity);
        await session.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<PersistedRequestEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var session = store.QuerySession();
        return await session.LoadAsync<PersistedRequestEntity>(id, ct);
    }

    public async Task<PersistedRequestEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default)
    {
        using var session = store.QuerySession();
        return await session.Query<PersistedRequestEntity>()
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
    }

    public async Task<PersistedRequestEntity> UpdateAsync(PersistedRequestEntity entity, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        session.Update(entity);
        await session.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> TryTakeOverAsync(Guid id, int expectedRetryCount, DateTimeOffset now, TimeSpan leaseTtl, CancellationToken ct = default)
    {
        // Ownership token вЂ” timestamp-only verification is unreliable: Postgres timestamptz
        // truncates sub-microsecond precision (exact-value equality can mismatch), and a
        // concurrent taker can compute the same lease timestamp. A per-takeover GUID makes the
        // win-check unambiguous.
        var leaseId = Guid.CreateVersion7();
        var leaseUntil = now + leaseTtl;

        using (var session = store.LightweightSession())
        {
            // IMPORTANT: each branch uses ONE chained patch (single UPDATE) вЂ” separate
            // Patch(filter) calls run as sequential UPDATEs and earlier ones invalidate the
            // WHERE of later ones (e.g. Statusв†’Processing makes `Status == Persisted` stop
            // matching), silently skipping the remaining sets.

            // Expired Processing lease вЂ” take over from the crashed/restarted execution. A live
            // holder has LeaseUntil in the future and never matches this filter. Null leases
            // (legacy/corrupt rows) are eligible too.
            var staleLeaseFilter = (System.Linq.Expressions.Expression<Func<PersistedRequestEntity, bool>>)
                (x => x.Id == id
                      && x.Status == RequestPersistenceStatus.Processing
                      && (x.LeaseUntil <= now || x.LeaseUntil == null));
            session.Patch(staleLeaseFilter)
                .Set(x => x.LeaseId, leaseId)
                .Set(x => x.LeaseUntil, leaseUntil)
                .Set(x => x.RetryCount, expectedRetryCount + 1)
                .Set(x => x.UpdatedAt, now);

            // Persisted (claimed but never executed вЂ” keeps RetryCount at its plain-attempt
            // value for the retention policy).
            var persistedFilter = (System.Linq.Expressions.Expression<Func<PersistedRequestEntity, bool>>)
                (x => x.Id == id && x.Status == RequestPersistenceStatus.Persisted);
            session.Patch(persistedFilter)
                .Set(x => x.Status, RequestPersistenceStatus.Processing)
                .Set(x => x.LeaseId, leaseId)
                .Set(x => x.LeaseUntil, leaseUntil)
                .Set(x => x.UpdatedAt, now);

            // Due RetryNeeded (bumps the recovery counter).
            var retryDueFilter = (System.Linq.Expressions.Expression<Func<PersistedRequestEntity, bool>>)
                (x => x.Id == id
                      && x.Status == RequestPersistenceStatus.RetryNeeded
                      && (x.NextAttemptAt == null || x.NextAttemptAt <= now));
            session.Patch(retryDueFilter)
                .Set(x => x.LeaseId, leaseId)
                .Set(x => x.LeaseUntil, leaseUntil)
                .Set(x => x.RetryCount, expectedRetryCount + 1)
                .Set(x => x.UpdatedAt, now);

            await session.SaveChangesAsync(ct);
        }

        // A takeover "win" is proven by OUR lease token living on the row.
        using var verify = store.QuerySession();
        var row = await verify.LoadAsync<PersistedRequestEntity>(id, ct);
        return row is not default(PersistedRequestEntity)
            && row.Status == RequestPersistenceStatus.Processing
            && row.LeaseId == leaseId;
    }

    public async Task MarkReplayFailureAsync(Guid id, string note, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        var row = await session.LoadAsync<PersistedRequestEntity>(id, ct);
        if (row is default(PersistedRequestEntity))
        {
            return;
        }

        var failed = row with
        {
            Status = RequestPersistenceStatus.Failed,
            LeaseUntil = null,
            Note = note,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        session.Update(failed);
        await session.SaveChangesAsync(ct);
    }

    public async Task AppendCheckpointAsync(RequestCheckpointEntity checkpoint, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        session.Insert(checkpoint);
        await session.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RequestCheckpointEntity>> GetCheckpointsAsync(Guid persistedRequestId, CancellationToken ct = default)
    {
        using var session = store.QuerySession();
        var checkpoints = await session.Query<RequestCheckpointEntity>()
            .Where(x => x.PersistedRequestId == persistedRequestId)
            .OrderBy(x => x.StepIndex)
            .ToListAsync(ct);
        return checkpoints;
    }

    public async Task<IReadOnlyList<PersistedRequestEntity>> GetRecoveryCandidatesAsync(DateTimeOffset now, TimeSpan recoverPersistedAfter, int limit, CancellationToken ct = default)
    {
        var persistedCutoff = now - recoverPersistedAfter;
        using var session = store.QuerySession();
        var candidates = await session.Query<PersistedRequestEntity>()
            .Where(x => (x.Status == RequestPersistenceStatus.Persisted && x.UpdatedAt <= persistedCutoff)
                        || (x.Status == RequestPersistenceStatus.RetryNeeded
                            && (x.NextAttemptAt == default || x.NextAttemptAt <= now))
                        || (x.Status == RequestPersistenceStatus.Processing && x.LeaseUntil != default && x.LeaseUntil <= now))
            .OrderBy(x => x.UpdatedAt)
            .Take(limit)
            .ToListAsync(ct);
        return candidates;
    }

    public async Task DeleteCheckpointsAsync(Guid persistedRequestId, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        var checkpoints = await session.Query<RequestCheckpointEntity>()
            .Where(x => x.PersistedRequestId == persistedRequestId)
            .ToListAsync(ct);
        foreach (var checkpoint in checkpoints)
        {
            session.Delete(checkpoint);
        }

        await session.SaveChangesAsync(ct);
    }

    public async Task TombstoneAsync(Guid id, DateTimeOffset completedAt, CancellationToken ct = default)
    {
        using var session = store.LightweightSession();
        var row = await session.LoadAsync<PersistedRequestEntity>(id, ct);
        if (row is default(PersistedRequestEntity))
        {
            return;
        }

        var tombstone = row with
        {
            // Keep only dedup identity: key + hash + terminal status + timestamps.
            Status = row.Status is RequestPersistenceStatus.Succeeded or RequestPersistenceStatus.Failed
                ? row.Status
                : RequestPersistenceStatus.Failed,
            RequestPayload = string.Empty,
            ResponsePayload = null,
            HeadersJson = null,
            ResponseStatusCode = null,
            Note = null,
            LeaseUntil = null,
            NextAttemptAt = null,
            UpdatedAt = completedAt
        };
        session.Update(tombstone);

        var checkpoints = await session.Query<RequestCheckpointEntity>()
            .Where(x => x.PersistedRequestId == id)
            .ToListAsync(ct);
        foreach (var checkpoint in checkpoints)
        {
            session.Delete(checkpoint);
        }

        await session.SaveChangesAsync(ct);
    }
}
