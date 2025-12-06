using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.NotificationsManagement.Repositories.Marten;

namespace snowcoreBlog.Backend.NotificationsManagement.Tests.Repositories;

public class NotificationRepositoryTests
{
    private readonly IDocumentSession _documentSession;
    private readonly NotificationRepository _repository;

    public NotificationRepositoryTests()
    {
        _documentSession = Substitute.For<IDocumentSession>();
        _repository = new NotificationRepository(_documentSession);
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
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.InsertAsync(notification);

        // Assert
        _documentSession.Received(1).Insert(notification);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.UpdateAsync(notification);

        // Assert
        _documentSession.Received(1).Update(notification);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDocumentSession()
    {
        // Arrange
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.DeleteAsync(notification);

        // Assert
        _documentSession.Received(1).Delete(notification);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
