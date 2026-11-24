using FluentAssertions;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.Infrastructure.Tests.Utilities;

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
