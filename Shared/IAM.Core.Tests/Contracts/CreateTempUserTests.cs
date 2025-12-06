using FluentAssertions;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Core.Tests.Contracts;

public class CreateTempUserTests
{
    [Fact]
    public void CreateTempUser_ShouldSetRequiredProperties()
    {
        // Arrange & Act
        var contract = new CreateTempUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            ConfirmedAgreement = true
        };

        // Assert
        contract.UserName.Should().Be("testuser");
        contract.Email.Should().Be("test@example.com");
        contract.FirstName.Should().Be("Test");
        contract.ConfirmedAgreement.Should().BeTrue();
    }

    [Fact]
    public void CreateTempUser_LastName_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var contract = new CreateTempUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            ConfirmedAgreement = true
        };

        // Assert
        contract.LastName.Should().Be(string.Empty);
    }

    [Fact]
    public void CreateTempUser_InitialEmailConsent_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var contract = new CreateTempUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            ConfirmedAgreement = true
        };

        // Assert
        contract.InitialEmailConsent.Should().BeTrue();
    }

    [Fact]
    public void CreateTempUser_PhoneNumber_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var contract = new CreateTempUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            ConfirmedAgreement = true
        };

        // Assert
        contract.PhoneNumber.Should().BeNull();
    }

    [Fact]
    public void CreateTempUser_ShouldAllowSettingOptionalProperties()
    {
        // Arrange & Act
        var contract = new CreateTempUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890",
            ConfirmedAgreement = true,
            InitialEmailConsent = false
        };

        // Assert
        contract.LastName.Should().Be("User");
        contract.PhoneNumber.Should().Be("+1234567890");
        contract.InitialEmailConsent.Should().BeFalse();
    }
}
