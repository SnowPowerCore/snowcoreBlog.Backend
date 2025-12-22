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
    public async Task AddOrUpdateAsync_ShouldStoreReader()
    {
        // Arrange
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NickName = "test"
        };

        // Act
        await _repository.AddOrUpdateAsync(reader);

        // Assert
        _documentSession.Received(1).Store(Arg.Is<ReaderEntity>(x => x.Id == reader.Id && x.UserId == reader.UserId));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithId_ShouldStoreReader()
    {
        // Arrange
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NickName = "test"
        };

        // Act
        await _repository.AddOrUpdateAsync(reader, reader.Id);

        // Assert
        _documentSession.Received(1).Store(Arg.Is<ReaderEntity>(x => x.Id == reader.Id && x.UserId == reader.UserId));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteReader()
    {
        // Arrange
        var reader = new ReaderEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NickName = "test"
        };

        // Act
        await _repository.RemoveAsync(reader);

        // Assert
        _documentSession.Received(1).Delete(reader);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
