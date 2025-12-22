using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.AuthorsManagement.Tests.Repositories;

public class AuthorRepositoryTests
{
    private readonly IDocumentSession _documentSession;
    private readonly AuthorRepository _repository;

    public AuthorRepositoryTests()
    {
        _documentSession = Substitute.For<IDocumentSession>();
        _repository = new AuthorRepository(_documentSession);
    }

    [Fact]
    public void Repository_ShouldBeCreated()
    {
        // Assert
        _repository.Should().NotBeNull();
    }

    [Fact]
    public async Task AddOrUpdateAsync_ShouldStoreAuthor()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DisplayName = "test"
        };

        // Act
        await _repository.AddOrUpdateAsync(author);

        // Assert
        _documentSession.Received(1).Store(Arg.Is<AuthorEntity>(x => x.Id == author.Id && x.UserId == author.UserId));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithId_ShouldStoreAuthor()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DisplayName = "test"
        };

        // Act
        await _repository.AddOrUpdateAsync(author, author.Id);

        // Assert
        _documentSession.Received(1).Store(Arg.Is<AuthorEntity>(x => x.Id == author.Id && x.UserId == author.UserId));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteAuthor()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DisplayName = "test"
        };

        // Act
        await _repository.RemoveAsync(author);

        // Assert
        _documentSession.Received(1).Delete(author);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
