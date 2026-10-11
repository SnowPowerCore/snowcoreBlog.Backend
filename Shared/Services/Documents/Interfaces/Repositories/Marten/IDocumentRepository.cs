using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;

public interface IDocumentRepository : IRepository<DocumentEntity>
{
    /// <summary>Finds the newest document with the given SHA-256 checksum (deduplication lookup).</summary>
    Task<DocumentEntity?> GetByChecksumAsync(string checksum, CancellationToken token = default);

    Task<IEnumerable<DocumentEntity>> GetByCategoryAsync(string category, CancellationToken token = default);
}
