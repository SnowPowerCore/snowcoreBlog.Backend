using Marten;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

public interface IIpRestrictionRepository
{
    Task<IReadOnlyList<IpRestrictionEntity>> GetAllAsync();
    Task<IpRestrictionEntity?> GetByIdAsync(Guid id);
    Task SaveAsync(IpRestrictionEntity entity);
}

public class IpRestrictionRepository(IDocumentStore store) : IIpRestrictionRepository
{
    public async Task<IReadOnlyList<IpRestrictionEntity>> GetAllAsync()
    {
        await using var s = store.LightweightSession();
        return await s.Query<IpRestrictionEntity>().ToListAsync();
    }

    public async Task<IpRestrictionEntity?> GetByIdAsync(Guid id)
    {
        await using var s = store.LightweightSession();
        return await s.LoadAsync<IpRestrictionEntity>(id);
    }

    public async Task SaveAsync(IpRestrictionEntity entity)
    {
        await using var s = store.LightweightSession();
        s.Store(entity);
        await s.SaveChangesAsync();
    }
}
