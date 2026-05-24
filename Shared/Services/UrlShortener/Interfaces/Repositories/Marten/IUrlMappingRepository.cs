using snowcoreBlog.Backend.UrlShortener.Core.Entities;

namespace snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;

public interface IUrlMappingRepository
{
    Task<UrlMappingEntity?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<UrlMappingEntity> AddOrUpdateAsync(UrlMappingEntity entity, Guid? id = null, bool saveChange = true, CancellationToken ct = default);
    Task RecordClickAsync(ClickEventEntity ev, CancellationToken ct = default);
    Task<ulong> CountClicksInWindowAsync(Guid mappingId, DateTime since, CancellationToken ct = default);
}
