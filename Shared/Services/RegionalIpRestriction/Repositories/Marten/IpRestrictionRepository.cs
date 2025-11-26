using Marten;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

public interface IIpRestrictionRepository
{
    Task<IReadOnlyList<IpRestrictionEntity>> GetAllAsync();
    Task<IpRestrictionEntity?> GetByIdAsync(Guid id);
    Task SaveAsync(IpRestrictionEntity entity);
}

public class IpRestrictionRepository : IIpRestrictionRepository
{
    private readonly IDocumentStore _store;

    public IpRestrictionRepository(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<IpRestrictionEntity>> GetAllAsync()
    {
        await using var s = _store.LightweightSession();
        return await s.Query<IpRestrictionEntity>().ToListAsync();
    }

    public async Task<IpRestrictionEntity?> GetByIdAsync(Guid id)
    {
        await using var s = _store.LightweightSession();
        return await s.LoadAsync<IpRestrictionEntity>(id);
    }

    public async Task SaveAsync(IpRestrictionEntity entity)
    {
        await using var s = _store.LightweightSession();
        s.Store(entity);
        await s.SaveChangesAsync();
    }
}
