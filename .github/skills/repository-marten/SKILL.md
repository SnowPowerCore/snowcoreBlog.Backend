Repository + Marten (persistence conventions)

Purpose
- Explain the repository pattern used with Marten in this backend and enforce using `Repositories/**` and `Interfaces/Repositories/**` for DB access.

Key rules
- Implement repository interfaces under `Interfaces/Repositories/Marten` and concrete classes under `Repositories/Marten`.
- Repository implementations should inherit from the shared `BaseMartenRepository<TEntity>` provided in infrastructure and inject `IDocumentSession`.
- Register interface/implementation pairs in `Program.cs` (e.g., `builder.Services.AddScoped<IReaderRepository, ReaderRepository>();`).

Example interface (Interfaces/Repositories/Marten/IReaderRepository.cs)
```csharp
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

public interface IReaderRepository : IRepository<ReaderEntity>
{
    // Add specialized queries here, e.g.:
    Task<ReaderEntity?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
```

Example implementation (Repositories/Marten/ReaderRepository.cs)
```csharp
using Marten;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

public class ReaderRepository(IDocumentSession session) : BaseMartenRepository<ReaderEntity>(session), IReaderRepository
{
    // Implement specialized queries that cannot be expressed by compiled queries or base methods.
}
```

DI registration (Program.cs)
```csharp
builder.Services.AddScoped<IReaderRepository, ReaderRepository>();
```

Best practices
- Use compiled queries when a query is used in many places (see `Core.Interfaces.CompiledQueries`).
- Keep repository methods small and focused; prefer returning domain entities or projections.
- Let the `BaseMartenRepository` handle common CRUD boilerplate (`GetByIdAsync`, `AddOrUpdateAsync`, `RemoveAsync`).
