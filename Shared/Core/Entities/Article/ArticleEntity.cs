using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Article;

public record ArticleEntity : BaseEntity
{
    // Support multiple authors; usually single author but allow many.
    public Guid[] AuthorUserIds { get; init; } = [];

    public required string Title { get; init; }

    public required string Slug { get; init; }

    // Latest snapshot id; this entity's Markdown is represented by the latest snapshot's content.
    public Guid LatestSnapshotId { get; init; }

    public DateTime? PublishedAt { get; init; }

    public string[] Tags { get; init; } = [];
    
    public string? CoverImageUrl { get; init; }
}