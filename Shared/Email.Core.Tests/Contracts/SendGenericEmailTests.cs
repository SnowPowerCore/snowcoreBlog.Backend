using FluentAssertions;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Core.Tests.Contracts;

public class SendGenericEmailTests
{
    [Fact]
    public void SendGenericEmail_ShouldSetRequiredProperties()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Assert
        email.SenderAddress.Should().Be("sender@example.com");
        email.ReceiverAddress.Should().Be("receiver@example.com");
        email.Subject.Should().Be("Test Subject");
    }

    [Fact]
    public void SendGenericEmail_SenderName_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Assert
        email.SenderName.Should().Be(string.Empty);
    }

    [Fact]
    public void SendGenericEmail_ReceiverName_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Assert
        email.ReceiverName.Should().Be(string.Empty);
    }

    [Fact]
    public void SendGenericEmail_PreHeader_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Assert
        email.PreHeader.Should().Be(string.Empty);
    }

    [Fact]
    public void SendGenericEmail_Content_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Assert
        email.Content.Should().Be(string.Empty);
    }

    [Fact]
    public void SendGenericEmail_ShouldAllowSettingAllProperties()
    {
        // Arrange & Act
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            SenderName = "Sender Name",
            ReceiverAddress = "receiver@example.com",
            ReceiverName = "Receiver Name",
            Subject = "Test Subject",
            PreHeader = "Preview text",
            Content = "Email content here"
        };

        // Assert
        email.SenderName.Should().Be("Sender Name");
        email.ReceiverName.Should().Be("Receiver Name");
        email.PreHeader.Should().Be("Preview text");
        email.Content.Should().Be("Email content here");
    }
}
