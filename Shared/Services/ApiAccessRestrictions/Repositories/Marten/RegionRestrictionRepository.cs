using Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

public interface IRegionRestrictionRepository
{
    Task<IReadOnlyList<RegionRestrictionEntity>> GetAllAsync(CancellationToken ct = default);
    Task<RegionRestrictionEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(RegionRestrictionEntity entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class RegionRestrictionRepository(IDocumentStore store) : IRegionRestrictionRepository
{
    public async Task<IReadOnlyList<RegionRestrictionEntity>> GetAllAsync(CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.Query<RegionRestrictionEntity>().ToListAsync(token: ct);
    }

    public async Task<RegionRestrictionEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.LoadAsync<RegionRestrictionEntity>(id, ct);
    }

    public async Task SaveAsync(RegionRestrictionEntity entity, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Store(entity);
        await s.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Delete<RegionRestrictionEntity>(id);
        await s.SaveChangesAsync(ct);
    }
}