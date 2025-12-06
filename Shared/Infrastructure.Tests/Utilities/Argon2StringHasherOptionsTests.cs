using FluentAssertions;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.Infrastructure.Tests.Utilities;

public class Argon2StringHasherOptionsTests
{
    [Fact]
    public void DefaultStrength_ShouldBeModerate()
    {
        // Arrange & Act
        var options = new Argon2StringHasherOptions();

        // Assert
        options.Strength.Should().Be(Argon2HashStrength.Moderate);
    }

    [Fact]
    public void Strength_ShouldBeSettable()
    {
        // Arrange
        var options = new Argon2StringHasherOptions();

        // Act
        options.Strength = Argon2HashStrength.Sensitive;

        // Assert
        options.Strength.Should().Be(Argon2HashStrength.Sensitive);
    }
}

public class Argon2HashStrengthTests
{
    [Fact]
    public void Interactive_ShouldHaveValueZero()
    {
        ((int)Argon2HashStrength.Interactive).Should().Be(0);
    }

    [Fact]
    public void Moderate_ShouldHaveValueOne()
    {
        ((int)Argon2HashStrength.Moderate).Should().Be(1);
    }

    [Fact]
    public void Sensitive_ShouldHaveValueTwo()
    {
        ((int)Argon2HashStrength.Sensitive).Should().Be(2);
    }
}
