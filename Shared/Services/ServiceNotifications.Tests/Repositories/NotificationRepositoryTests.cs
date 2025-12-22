using FluentAssertions;
using Marten;
using NSubstitute;
using snowcoreBlog.Backend.Core.Entities.Notification;
using snowcoreBlog.Backend.ServiceNotifications.Repositories.Marten;

namespace snowcoreBlog.Backend.ServiceNotifications.Tests.Repositories;

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
    public async Task AddOrUpdateAsync_ShouldCallStoreAndSaveChanges()
    {
        // Arrange
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Type = NotificationType.Info,
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddOrUpdateAsync(notification);

        // Assert
        _documentSession.Received(1).Store(notification);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithIdOverride_ShouldStoreWithThatId()
    {
        // Arrange
        var idOverride = Guid.NewGuid();
        var notification = new NotificationEntity
        {
            // Note: base repo should override the id when idOverride is provided
            Id = Guid.NewGuid(),
            Title = "Test",
            Type = NotificationType.Info,
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddOrUpdateAsync(notification, id: idOverride);

        // Assert
        result.Id.Should().Be(idOverride);
        _documentSession.Received(1).Store(Arg.Is<NotificationEntity>(n => n.Id == idOverride));
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallDeleteAndSaveChanges()
    {
        // Arrange
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Type = NotificationType.Info,
            IsActive = true,
            Priority = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.RemoveAsync(notification);

        // Assert
        _documentSession.Received(1).Delete(notification);
        await _documentSession.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
