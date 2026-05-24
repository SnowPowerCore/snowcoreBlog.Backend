using Marten;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.Infrastructure.Repositories;

public class RequestPersistenceRepository(IDocumentSession session) : BaseMartenRepository<PersistedRequestEntity>(session), IRequestPersistenceRepository
{
    private readonly IDocumentSession _session = session;

    public async Task<PersistedRequestEntity> CreateAsync(PersistedRequestEntity entity, CancellationToken ct = default)
    {
        var id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        var toStore = entity with
        {
            Id = id,
            RequestId = id,
            CreatedAt = entity.CreatedAt == default ? DateTimeOffset.UtcNow : entity.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var stored = await AddOrUpdateAsync(toStore, id, saveChange: true, ct);

        return stored;
    }

    public async Task<PersistedRequestEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return null;

        return await _session.Query<PersistedRequestEntity>().FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
    }

    public async Task UpdateAsync(PersistedRequestEntity entity, CancellationToken ct = default)
    {
        await AddOrUpdateAsync(entity, entity.Id, saveChange: true, ct);
    }

    public async Task UpdateStatusAsync(Guid id, RequestPersistenceStatus status, string? responsePayload = null, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing is default(PersistedRequestEntity))
            return;

        var retryCount = existing.RetryCount;
        if (status == RequestPersistenceStatus.RetryNeeded)
            retryCount++;

        var updated = existing with
        {
            Status = status,
            ResponsePayload = responsePayload ?? existing.ResponsePayload,
            UpdatedAt = DateTimeOffset.UtcNow,
            RetryCount = retryCount
        };

        await AddOrUpdateAsync(updated, id, saveChange: true, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing is default(PersistedRequestEntity))
            return;

        await RemoveAsync(existing, true, ct);
    }
}