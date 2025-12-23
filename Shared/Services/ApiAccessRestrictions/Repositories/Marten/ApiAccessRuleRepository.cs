using Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

public interface IApiAccessRuleRepository
{
    Task<IReadOnlyList<ApiAccessRuleEntity>> GetAllAsync(CancellationToken ct = default);
    Task<ApiAccessRuleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(ApiAccessRuleEntity entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class ApiAccessRuleRepository(IDocumentStore store) : IApiAccessRuleRepository
{
    public async Task<IReadOnlyList<ApiAccessRuleEntity>> GetAllAsync(CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.Query<ApiAccessRuleEntity>().ToListAsync(token: ct);
    }

    public async Task<ApiAccessRuleEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.LoadAsync<ApiAccessRuleEntity>(id, ct);
    }

    public async Task SaveAsync(ApiAccessRuleEntity entity, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Store(entity);
        await s.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Delete<ApiAccessRuleEntity>(id);
        await s.SaveChangesAsync(ct);
    }
}