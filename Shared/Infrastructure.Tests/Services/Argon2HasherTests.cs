using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.Infrastructure.Tests.Services;

public class Argon2HasherTests
{
    private readonly Argon2Hasher _sut;

    public Argon2HasherTests()
    {
        var options = Options.Create(new Argon2StringHasherOptions
        {
            Strength = Argon2HashStrength.Interactive // Using Interactive for faster tests
        });
        _sut = new Argon2Hasher(options);
    }

    [Fact]
    public void Hash_WithValidString_ShouldReturnHashedValue()
    {
        // Arrange
        var target = "test-string-to-hash";

        // Act
        var result = _sut.Hash(target);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(target);
        result.Should().StartWith("$argon2");
    }

    [Fact]
    public void Hash_WithSameInput_ShouldReturnDifferentHashes()
    {
        // Arrange
        var target = "test-string";

        // Act
        var hash1 = _sut.Hash(target);
        var hash2 = _sut.Hash(target);

        // Assert
        hash1.Should().NotBe(hash2, "Argon2 uses random salt");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Hash_WithNullOrWhitespace_ShouldThrowArgumentNullException(string? target)
    {
        // Act
        var act = () => _sut.Hash(target!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifyHashedStrings_WithMatchingStrings_ShouldReturnSuccess()
    {
        // Arrange
        var original = "my-secret-string";
        var hashed = _sut.Hash(original);

        // Act
        var result = _sut.VerifyHashedStrings(hashed, original);

        // Assert
        result.Should().Be(HashedStringsVerificationResult.Success);
    }

    [Fact]
    public void VerifyHashedStrings_WithNonMatchingStrings_ShouldReturnFailed()
    {
        // Arrange
        var original = "my-secret-string";
        var hashed = _sut.Hash(original);
        var wrongString = "wrong-string";

        // Act
        var result = _sut.VerifyHashedStrings(hashed, wrongString);

        // Assert
        result.Should().Be(HashedStringsVerificationResult.Failed);
    }

    [Theory]
    [InlineData(null, "target")]
    [InlineData("", "target")]
    [InlineData("   ", "target")]
    public void VerifyHashedStrings_WithNullOrWhitespaceHashedString_ShouldThrowArgumentNullException(
        string? hashedString, string targetString)
    {
        // Act
        var act = () => _sut.VerifyHashedStrings(hashedString!, targetString);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("hashed", null)]
    [InlineData("hashed", "")]
    [InlineData("hashed", "   ")]
    public void VerifyHashedStrings_WithNullOrWhitespaceTargetString_ShouldThrowArgumentNullException(
        string hashedString, string? targetString)
    {
        // Act
        var act = () => _sut.VerifyHashedStrings(hashedString, targetString!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldUseDefaultStrength()
    {
        // Arrange & Act
        var hasher = new Argon2Hasher(null);
        var hash = hasher.Hash("test");

        // Assert
        hash.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(Argon2HashStrength.Interactive)]
    [InlineData(Argon2HashStrength.Moderate)]
    [InlineData(Argon2HashStrength.Sensitive)]
    public void Hash_WithDifferentStrengths_ShouldWork(Argon2HashStrength strength)
    {
        // Arrange
        var options = Options.Create(new Argon2StringHasherOptions { Strength = strength });
        var hasher = new Argon2Hasher(options);
        var target = "test-string";

        // Act
        var hash = hasher.Hash(target);
        var result = hasher.VerifyHashedStrings(hash, target);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        result.Should().Be(HashedStringsVerificationResult.Success);
    }
}
