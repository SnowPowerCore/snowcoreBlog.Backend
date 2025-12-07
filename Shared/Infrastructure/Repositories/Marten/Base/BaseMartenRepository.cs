using Marten;
using Marten.Linq;
using snowcoreBlog.Backend.Core.Base;
using snowcoreBlog.Backend.Core.Interfaces.CompiledQueries;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

public class BaseMartenRepository<TEntity>(IDocumentSession session) : IRepository<TEntity> where TEntity : notnull, BaseEntity
{
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default) => await session.Query<TEntity>().ToListAsync(token);

    public Task<IEnumerable<TEntity>> GetAllByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default) =>
        session.QueryAsync(queryProvider.GetQuery<ICompiledListQuery<TEntity, TEntity>>(), token);

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default) =>
        session.LoadAsync<TEntity>(id, token);

    public Task<TEntity?> GetOneByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default) =>
        session.QueryAsync(queryProvider.GetQuery<ICompiledQuery<TEntity, TEntity?>>(), token);

    public Task<bool> AnyByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default) =>
        session.QueryAsync(queryProvider.GetQuery<ICompiledQuery<TEntity, bool>>(), token);

    public Task<ulong> CountByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default) =>
        session.QueryAsync(queryProvider.GetQuery<ICompiledQuery<TEntity, ulong>>(), token);

    public async Task<TEntity> AddOrUpdateAsync(TEntity entity, Guid? id = null, bool saveChange = true, CancellationToken token = default)
    {
        var addUpdateEntity = id is not null ? entity with { Id = id.Value } : entity;

        session.Store(addUpdateEntity);

        if (saveChange)
            await session.SaveChangesAsync(token);

        return addUpdateEntity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, bool saveChange = true, CancellationToken token = default)
    {
        session.Store(entities);

        if (saveChange)
            await session.SaveChangesAsync(token);

        return entities;
    }

    public Task RemoveAsync(TEntity entity, bool saveChange = true, CancellationToken token = default)
    {
        session.Delete(entity);

        return saveChange ? session.SaveChangesAsync(token) : Task.CompletedTask;
    }
}