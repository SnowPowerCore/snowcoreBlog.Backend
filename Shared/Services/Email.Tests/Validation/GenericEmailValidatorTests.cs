using FluentAssertions;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Validation;

namespace snowcoreBlog.Backend.Email.Tests.Validation;

public class GenericEmailValidatorTests
{
    private readonly GenericEmailValidator _validator;

    public GenericEmailValidatorTests()
    {
        _validator = new GenericEmailValidator();
    }

    [Fact]
    public void Validate_WithValidEmails_ShouldPass()
    {
        // Arrange
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidSenderAddress_ShouldFail(string senderAddress)
    {
        // Arrange
        var email = new SendGenericEmail
        {
            SenderAddress = senderAddress,
            ReceiverAddress = "receiver@example.com",
            Subject = "Test Subject"
        };

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SenderAddress");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidReceiverAddress_ShouldFail(string receiverAddress)
    {
        // Arrange
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = receiverAddress,
            Subject = "Test Subject"
        };

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ReceiverAddress");
    }

    [Fact]
    public void Validate_WithEmptySubject_ShouldFail()
    {
        // Arrange
        var email = new SendGenericEmail
        {
            SenderAddress = "sender@example.com",
            ReceiverAddress = "receiver@example.com",
            Subject = ""
        };

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Validate_WithAllInvalidFields_ShouldReturnMultipleErrors()
    {
        // Arrange
        var email = new SendGenericEmail
        {
            SenderAddress = "invalid",
            ReceiverAddress = "invalid",
            Subject = ""
        };

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
    }
}
