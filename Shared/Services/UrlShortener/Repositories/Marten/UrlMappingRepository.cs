using Marten;
using snowcoreBlog.Backend.UrlShortener.Core.Entities;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.UrlShortener.Repositories.Marten;

public class UrlMappingRepository(IDocumentSession session) : BaseMartenRepository<UrlMappingEntity>(session), IUrlMappingRepository
{
    public Task<UrlMappingEntity?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        session.Query<UrlMappingEntity>().Where(x => x.Code == code).FirstOrDefaultAsync(ct);

    public async Task RecordClickAsync(ClickEventEntity ev, CancellationToken ct = default)
    {
        session.Store(ev);
        await session.SaveChangesAsync(ct);
    }

    public async Task<ulong> CountClicksInWindowAsync(Guid mappingId, DateTime since, CancellationToken ct = default)
    {
        var count = await session.Query<ClickEventEntity>().Where(e => e.UrlMappingId == mappingId && e.ClickedAt >= since).CountAsync(ct);
        return (ulong)count;
    }

    public new Task<UrlMappingEntity> AddOrUpdateAsync(UrlMappingEntity entity, Guid? id = null, bool saveChange = true, CancellationToken ct = default) =>
        base.AddOrUpdateAsync(entity, id, saveChange, ct);
}
