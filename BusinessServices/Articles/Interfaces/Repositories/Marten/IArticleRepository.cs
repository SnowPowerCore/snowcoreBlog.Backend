using snowcoreBlog.Backend.Core.Entities.Article;

namespace snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;

public interface IArticleRepository
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task<Guid> InsertArticleWithSnapshotAsync(ArticleEntity entity, ArticleSnapshotEntity snapshot, CancellationToken ct = default);
    Task InsertSnapshotAsync(ArticleSnapshotEntity snapshot, CancellationToken ct = default);
    Task<ArticleSnapshotEntity?> GetLatestSnapshotAsync(Guid articleId, CancellationToken ct = default);
}
