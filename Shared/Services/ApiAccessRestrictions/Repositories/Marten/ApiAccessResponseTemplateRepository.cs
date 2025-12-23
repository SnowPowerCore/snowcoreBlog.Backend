using Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

public interface IApiAccessResponseTemplateRepository
{
    Task<IReadOnlyList<ApiAccessResponseTemplateEntity>> GetAllAsync(CancellationToken ct = default);
    Task<ApiAccessResponseTemplateEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(ApiAccessResponseTemplateEntity entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class ApiAccessResponseTemplateRepository(IDocumentStore store) : IApiAccessResponseTemplateRepository
{
    public async Task<IReadOnlyList<ApiAccessResponseTemplateEntity>> GetAllAsync(CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.Query<ApiAccessResponseTemplateEntity>().ToListAsync(token: ct);
    }

    public async Task<ApiAccessResponseTemplateEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        return await s.LoadAsync<ApiAccessResponseTemplateEntity>(id, ct);
    }

    public async Task SaveAsync(ApiAccessResponseTemplateEntity entity, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Store(entity);
        await s.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var s = store.LightweightSession();
        s.Delete<ApiAccessResponseTemplateEntity>(id);
        await s.SaveChangesAsync(ct);
    }
}