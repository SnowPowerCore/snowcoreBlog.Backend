using Marten;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Article;

namespace snowcoreBlog.Backend.Articles.Repositories.Marten;

public class ArticleRepository(IQuerySession querySession, IDocumentSession docSession) : IArticleRepository
{
    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        querySession.Query<ArticleEntity>().AnyAsync(x => x.Slug == slug, ct);

    public async Task<Guid> InsertArticleWithSnapshotAsync(ArticleEntity entity, ArticleSnapshotEntity snapshot, CancellationToken ct = default)
    {
        // Insert snapshot and article; article.LatestSnapshotId should reference snapshot.Id
        docSession.Insert(snapshot);
        docSession.Insert(entity);
        await docSession.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task InsertSnapshotAsync(ArticleSnapshotEntity snapshot, CancellationToken ct = default)
    {
        docSession.Insert(snapshot);
        await docSession.SaveChangesAsync(ct);
    }

    public Task<ArticleSnapshotEntity?> GetLatestSnapshotAsync(Guid articleId, CancellationToken ct = default) =>
        querySession.Query<ArticleSnapshotEntity>().Where(x => x.ArticleId == articleId).OrderByDescending(x => x.ModifiedAt).FirstOrDefaultAsync(ct);
}
