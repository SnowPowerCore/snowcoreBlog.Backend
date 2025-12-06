using FluentAssertions;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Core.Tests.Entities;

public class ApplicationUserEntityTests
{
    [Fact]
    public void ApplicationUserEntity_PublicKeyCredentials_ShouldInitializeAsEmptyCollection()
    {
        // Arrange & Act
        var entity = new ApplicationUserEntity();

        // Assert
        entity.PublicKeyCredentials.Should().NotBeNull();
        entity.PublicKeyCredentials.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUserEntity_ShouldAllowSettingProperties()
    {
        // Arrange
        var entity = new ApplicationUserEntity();
        var credentialId = Guid.NewGuid();

        // Act
        entity.FirstName = "John";
        entity.LastName = "Doe";
        entity.Email = "john.doe@example.com";
        entity.UserName = "johndoe";
        entity.RoleClaims = new List<string> { "Reader", "Author" };
        entity.PublicKeyCredentials.Add(credentialId);

        // Assert
        entity.FirstName.Should().Be("John");
        entity.LastName.Should().Be("Doe");
        entity.Email.Should().Be("john.doe@example.com");
        entity.UserName.Should().Be("johndoe");
        entity.RoleClaims.Should().Contain("Reader");
        entity.RoleClaims.Should().Contain("Author");
        entity.PublicKeyCredentials.Should().Contain(credentialId);
    }

    [Fact]
    public void ApplicationUserEntity_ShouldInheritFromIdentityUser()
    {
        // Arrange & Act
        var entity = new ApplicationUserEntity();

        // Assert
        entity.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser>();
    }

    [Fact]
    public void ApplicationUserEntity_ShouldHaveIdProperty()
    {
        // Arrange
        var entity = new ApplicationUserEntity();
        var expectedId = Guid.NewGuid().ToString();

        // Act
        entity.Id = expectedId;

        // Assert
        entity.Id.Should().Be(expectedId);
    }
}
