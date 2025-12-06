using FluentAssertions;
using snowcoreBlog.Backend.Push.Core.Contracts;
using snowcoreBlog.Backend.Push.Validation;

namespace snowcoreBlog.Backend.Push.Tests.Validation;

public class GenericPushValidatorTests
{
    private readonly GenericPushValidator _validator;

    public GenericPushValidatorTests()
    {
        _validator = new GenericPushValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject"
        };

        // Act
        var result = _validator.Validate(push);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTopic_ShouldFail()
    {
        // Arrange
        var push = new SendGenericPush
        {
            Topic = "",
            Subject = "Test Subject"
        };

        // Act
        var result = _validator.Validate(push);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Topic");
    }

    [Fact]
    public void Validate_WithEmptySubject_ShouldFail()
    {
        // Arrange
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = ""
        };

        // Act
        var result = _validator.Validate(push);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Validate_WithAllInvalidFields_ShouldReturnMultipleErrors()
    {
        // Arrange
        var push = new SendGenericPush
        {
            Topic = "",
            Subject = ""
        };

        // Act
        var result = _validator.Validate(push);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Theory]
    [InlineData(NotificationPriority.Min)]
    [InlineData(NotificationPriority.Low)]
    [InlineData(NotificationPriority.Default)]
    [InlineData(NotificationPriority.High)]
    [InlineData(NotificationPriority.Max)]
    public void Validate_WithAllPriorities_ShouldPass(NotificationPriority priority)
    {
        // Arrange
        var push = new SendGenericPush
        {
            Topic = "test-topic",
            Subject = "Test Subject",
            Priority = priority
        };

        // Act
        var result = _validator.Validate(push);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
