using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.Backend.Documents.Repositories.Marten;

namespace snowcoreBlog.Backend.Documents.Tests.Repositories;

public class DocumentRepositoryTests
{
    private readonly IDocumentSession _documentSession;
    private readonly DocumentRepository _repository;

    public DocumentRepositoryTests()
    {
        _documentSession = Substitute.For<IDocumentSession>();
        _repository = new DocumentRepository(_documentSession);
    }

    private static DocumentEntity CreateEntity() => new()
    {
        Id = Guid.NewGuid(),
        FileName = "test.pdf",
        ContentType = "application/pdf",
        SizeBytes = 128,
        Category = "general",
        Status = DocumentStatus.Processed,
        Checksum = "abc123",
        ContentKey = "abc123",
        UploadedAt = DateTime.UtcNow,
        ProcessedAt = DateTime.UtcNow
    };

    [Fact]
    public void Repository_ShouldBeCreated()
    {
        _repository.Should().NotBeNull();
    }

    [Fact]
    public async Task AddOrUpdateAsync_ShouldCallStoreAndSaveChanges()
    {
        // Arrange
        var document = CreateEntity();

        // Act
        await _repository.AddOrUpdateAsync(document);

        // Assert
        _documentSession.Received(1).Store(document);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithIdOverride_ShouldStoreWithThatId()
    {
        // Arrange
        var idOverride = Guid.NewGuid();
        var document = CreateEntity();

        // Act
        var result = await _repository.AddOrUpdateAsync(document, id: idOverride);

        // Assert
        result.Id.Should().Be(idOverride);
        _documentSession.Received(1).Store(Arg.Is<DocumentEntity>(d => d.Id == idOverride));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallDeleteAndSaveChanges()
    {
        // Arrange
        var document = CreateEntity();

        // Act
        await _repository.RemoveAsync(document);

        // Assert
        _documentSession.Received(1).Delete(document);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
