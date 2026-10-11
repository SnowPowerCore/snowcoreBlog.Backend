using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using Marten;

namespace snowcoreBlog.Backend.Documents.Repositories.Marten;

public class DocumentRepository(IDocumentSession session) : BaseMartenRepository<DocumentEntity>(session), IDocumentRepository
{
    private readonly IDocumentSession _session = session;

    public Task<DocumentEntity?> GetByChecksumAsync(string checksum, CancellationToken token = default) =>
        _session.Query<DocumentEntity>()
            .Where(d => d.Checksum == checksum)
            .OrderByDescending(d => d.UploadedAt)
            .FirstOrDefaultAsync(token);

    public async Task<IEnumerable<DocumentEntity>> GetByCategoryAsync(string category, CancellationToken token = default) =>
        await _session.Query<DocumentEntity>()
            .Where(d => d.Category == category)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(token);
}
