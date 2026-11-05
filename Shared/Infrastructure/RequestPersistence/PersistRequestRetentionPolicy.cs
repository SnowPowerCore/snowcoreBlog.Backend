using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// Terminal-state cleanup: plain first-attempt finishes purge to dedup tombstones; requests that
/// went through recovery keep their full record for audit.
/// </summary>
public static class PersistRequestRetentionPolicy
{
    public static async Task ApplyAsync(
        IRequestPersistenceRepository repository,
        PersistRequestOptions options,
        PersistedRequestEntity finished,
        DateTimeOffset now,
        CancellationToken ct)
    {
        if (finished.Status is not (RequestPersistenceStatus.Succeeded or RequestPersistenceStatus.Failed))
        {
            return;
        }

        var plainFirstAttempt = finished.RetryCount == 0;
        var shouldTombstone = (plainFirstAttempt && options.TombstonePlainFinishes)
                              || (!plainFirstAttempt && !options.KeepRecoveredForAudit);

        if (shouldTombstone)
        {
            await repository.TombstoneAsync(finished.Id, now, ct);
        }
    }
}
