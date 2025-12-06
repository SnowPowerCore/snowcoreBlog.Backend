using FluentAssertions;
using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Tests.Base;

public class BaseEntityTests
{
    private record TestEntity() : BaseEntity();

    [Fact]
    public void BaseEntity_ShouldHaveDefaultGuid_WhenCreated()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void BaseEntity_ShouldAllowSettingId_WhenInitialized()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var entity = new TestEntity { Id = expectedId };

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void BaseEntity_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id };
        var entity2 = new TestEntity { Id = id };

        // Assert
        entity1.Should().Be(entity2);
    }

    [Fact]
    public void BaseEntity_WithDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        // Assert
        entity1.Should().NotBe(entity2);
    }
}
