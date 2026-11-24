using FluentAssertions;
using snowcoreBlog.Backend.Core.Utilities;

namespace snowcoreBlog.Backend.Core.Tests.Utilities;

public class HashedStringsVerificationResultTests
{
    [Fact]
    public void HashedStringsVerificationResult_Failed_ShouldHaveValueZero()
    {
        // Arrange & Act
        var result = HashedStringsVerificationResult.Failed;

        // Assert
        ((int)result).Should().Be(0);
    }

    [Fact]
    public void HashedStringsVerificationResult_Success_ShouldHaveValueOne()
    {
        // Arrange & Act
        var result = HashedStringsVerificationResult.Success;

        // Assert
        ((int)result).Should().Be(1);
    }

    [Fact]
    public void HashedStringsVerificationResult_SuccessRehashNeeded_ShouldHaveValueTwo()
    {
        // Arrange & Act
        var result = HashedStringsVerificationResult.SuccessRehashNeeded;

        // Assert
        ((int)result).Should().Be(2);
    }

    [Fact]
    public void HashedStringsVerificationResult_ShouldHaveThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<HashedStringsVerificationResult>();

        // Assert
        values.Should().HaveCount(3);
    }
}
