using snowcoreBlog.Backend.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;

public interface IUrlMappingRepository : IRepository<UrlMappingEntity>
{
    Task<UrlMappingEntity?> GetByCodeAsync(string code, CancellationToken ct = default);

    Task RecordClickAsync(ClickEventEntity ev, CancellationToken ct = default);
    
    Task<ulong> CountClicksInWindowAsync(Guid mappingId, DateTime since, CancellationToken ct = default);
}
