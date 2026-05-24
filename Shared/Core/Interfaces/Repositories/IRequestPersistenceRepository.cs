using snowcoreBlog.Backend.Core.Entities.RequestPersistence;

namespace snowcoreBlog.Backend.Core.Interfaces.Repositories;

public interface IRequestPersistenceRepository
{
    Task<PersistedRequestEntity> CreateAsync(PersistedRequestEntity entity, CancellationToken ct = default);

    Task<PersistedRequestEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PersistedRequestEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default);

    Task UpdateAsync(PersistedRequestEntity entity, CancellationToken ct = default);

    Task UpdateStatusAsync(Guid id, RequestPersistenceStatus status, string? responsePayload = null, CancellationToken ct = default);
    
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}