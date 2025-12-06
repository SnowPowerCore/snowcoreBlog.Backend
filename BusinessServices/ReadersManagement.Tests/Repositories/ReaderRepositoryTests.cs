using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;

namespace snowcoreBlog.Backend.ReadersManagement.Tests.Repositories;

public class ReaderRepositoryTests
{
    private readonly IDocumentSession _documentSession;
    private readonly ReaderRepository _repository;

    public ReaderRepositoryTests()
    {
        _documentSession = Substitute.For<IDocumentSession>();
        _repository = new ReaderRepository(_documentSession);
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
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.InsertAsync(reader);

        // Assert
        _documentSession.Received(1).Insert(reader);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.UpdateAsync(reader);

        // Assert
        _documentSession.Received(1).Update(reader);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _repository.DeleteAsync(reader);

        // Assert
        _documentSession.Received(1).Delete(reader);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
