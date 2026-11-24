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
