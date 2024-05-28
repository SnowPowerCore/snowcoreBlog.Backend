using snowcoreBlog.Backend.Core.Base;
using snowcoreBlog.Backend.Core.Interfaces.CompiledQueries;

namespace snowcoreBlog.Backend.Core.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : notnull, BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default);

    Task<IEnumerable<TEntity>> GetAllByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<TEntity?> GetOneByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default);

    Task<bool> AnyByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default);

    Task<ulong> CountByQueryAsync(ICompiledQueriesProvider queryProvider, CancellationToken token = default);

    Task<TEntity> AddOrUpdateAsync(TEntity entity, Guid? id = null, bool saveChange = true, CancellationToken token = default);

    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, bool saveChange = true, CancellationToken token = default);

    Task RemoveAsync(TEntity entity, bool saveChange = true, CancellationToken token = default);
}