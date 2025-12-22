using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.Articles.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Article;

namespace snowcoreBlog.Backend.Articles.Tests.Repositories;

public class ArticleRepositoryTests
{
    private readonly IQuerySession _querySession;
    private readonly IDocumentSession _documentSession;
    private readonly ArticleRepository _repository;

    public ArticleRepositoryTests()
    {
        _querySession = Substitute.For<IQuerySession>();
        _documentSession = Substitute.For<IDocumentSession>();
        _repository = new ArticleRepository(_querySession, _documentSession);
    }

    [Fact]
    public async Task InsertArticleWithSnapshotAsync_ShouldInsertBothEntities()
    {
        // Arrange
        var article = new ArticleEntity 
        { 
            Id = Guid.NewGuid(),
            Title = "Test Article",
            Slug = "test-article",
            PublishedAt = DateTime.UtcNow
        };
        var snapshot = new ArticleSnapshotEntity 
        { 
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            Markdown = "# Test",
            ModifiedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.InsertArticleWithSnapshotAsync(article, snapshot);

        // Assert
        result.Should().Be(article.Id);
        _documentSession.Received(1).Insert(snapshot);
        _documentSession.Received(1).Insert(article);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InsertSnapshotAsync_ShouldInsertSnapshot()
    {
        // Arrange
        var snapshot = new ArticleSnapshotEntity 
        { 
            Id = Guid.NewGuid(),
            ArticleId = Guid.NewGuid(),
            Markdown = "# Test",
            ModifiedAt = DateTime.UtcNow
        };

        // Act
        await _repository.InsertSnapshotAsync(snapshot);

        // Assert
        _documentSession.Received(1).Insert(snapshot);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
