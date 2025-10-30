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

    public async Task<List<(ArticleEntity Article, ArticleSnapshotEntity? Snapshot)>> GetLatestArticlesWithSnapshotsAsync(int count, CancellationToken ct = default)
    {
        var articles = await querySession.Query<ArticleEntity>()
            .OrderByDescending(x => x.PublishedAt)
            .Take(count)
            .ToListAsync(ct);

        if (articles.Count == 0)
            return [];

        var articleIds = articles.Select(a => a.Id).ToArray();

        var snapshots = await querySession.Query<ArticleSnapshotEntity>()
            .Where(s => articleIds.Contains(s.ArticleId))
            .ToListAsync(ct);

        // Pick latest snapshot per article
        var latestByArticle = snapshots
            .GroupBy(s => s.ArticleId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.ModifiedAt).First());

        return articles.Select(a => (a, latestByArticle.TryGetValue(a.Id, out var snap) ? snap : null)).ToList();
    }
}