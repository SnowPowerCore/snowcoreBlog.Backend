using FluentAssertions;
using snowcoreBlog.Backend.Push.Core.Contracts;

namespace snowcoreBlog.Backend.Push.Core.Tests.Contracts;

public class SendGenericPushTests
{
    [Fact]
    public void SendGenericPush_ShouldSetRequiredProperties()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Topic.Should().Be("test-topic");
        push.Subject.Should().Be("Test Subject");
    }

    [Fact]
    public void SendGenericPush_Content_ShouldDefaultToEmptyString()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Content.Should().Be(string.Empty);
    }

    [Fact]
    public void SendGenericPush_IsContentMarkdown_ShouldDefaultToFalse()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.IsContentMarkdown.Should().BeFalse();
    }

    [Fact]
    public void SendGenericPush_Priority_ShouldDefaultToDefault()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Priority.Should().Be(NotificationPriority.Default);
    }

    [Fact]
    public void SendGenericPush_Actions_ShouldDefaultToEmptyList()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Actions.Should().NotBeNull();
        push.Actions.Should().BeEmpty();
    }

    [Fact]
    public void SendGenericPush_Tags_ShouldDefaultToEmptyList()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Tags.Should().NotBeNull();
        push.Tags.Should().BeEmpty();
    }

    [Fact]
    public void SendGenericPush_Attachment_ShouldDefaultToNull()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Assert
        push.Attachment.Should().BeNull();
    }

    [Fact]
    public void SendGenericPush_ShouldAllowSettingAllProperties()
    {
        // Arrange & Act
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject",
            Content = "Push content",
            IsContentMarkdown = true,
            IconUri = "https://example.com/icon.png",
            Email = "test@example.com",
            PhoneNumber = "+1234567890",
            Priority = NotificationPriority.High,
            ClickUri = "https://example.com",
            Tags = new List<string> { "tag1", "tag2" }
        };

        // Assert
        push.Content.Should().Be("Push content");
        push.IsContentMarkdown.Should().BeTrue();
        push.IconUri.Should().Be("https://example.com/icon.png");
        push.Email.Should().Be("test@example.com");
        push.PhoneNumber.Should().Be("+1234567890");
        push.Priority.Should().Be(NotificationPriority.High);
        push.ClickUri.Should().Be("https://example.com");
        push.Tags.Should().HaveCount(2);
    }
}
