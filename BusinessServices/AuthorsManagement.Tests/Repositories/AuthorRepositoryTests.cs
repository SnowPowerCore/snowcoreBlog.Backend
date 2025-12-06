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
    public async Task InsertAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.InsertAsync(author);

        // Assert
        _documentSession.Received(1).Insert(author);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.UpdateAsync(author);

        // Assert
        _documentSession.Received(1).Update(author);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var author = new AuthorEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.DeleteAsync(author);

        // Assert
        _documentSession.Received(1).Delete(author);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
