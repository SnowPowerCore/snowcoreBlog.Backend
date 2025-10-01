using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Article;

public record ArticleSnapshotEntity : BaseEntity
{
    public required Guid ArticleId { get; init; }
    public required string Markdown { get; init; }
    public required DateTime ModifiedAt { get; init; }
}
