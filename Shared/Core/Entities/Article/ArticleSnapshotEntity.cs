using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Article;

public record ArticleSnapshotEntity : BaseEntity
{
    public required Guid ArticleId { get; init; }

    public required string Markdown { get; init; }

    // Short summary shown in lists; null = derive from content on the client.
    public string? Excerpt { get; init; }

    // SEO meta description (rendered into <meta name="description">); null = fall back to Excerpt.
    public string? MetaDescription { get; init; }
    
    public required DateTime ModifiedAt { get; init; }
}
