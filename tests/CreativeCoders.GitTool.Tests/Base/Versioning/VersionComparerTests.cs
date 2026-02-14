using CreativeCoders.GitTool.Base.Versioning;
using AwesomeAssertions;
using Xunit;

namespace CreativeCoders.GitTool.Tests.Base.Versioning;

public class VersionComparerTests
{
    [Theory]
    [InlineData("1.0.0", "1.0.0", 0)]
    [InlineData("2.0.0", "1.0.0", 1)]
    [InlineData("1.0.0", "2.0.0", -1)]
    [InlineData("1.1.0", "1.0.0", 1)]
    [InlineData("1.0.0", "1.1.0", -1)]
    [InlineData("1.0.1", "1.0.0", 1)]
    [InlineData("1.0.0", "1.0.1", -1)]
    [InlineData("1.1", "1.1.0", 0)]
    [InlineData("1", "1.0.0", 0)]
    [InlineData("1.2", "1.1", 1)]
    [InlineData("1.1", "1.2", -1)]
    [InlineData("1.10", "1.2", 1)]
    public void Compare_PositiveCases_ReturnsExpectedResult(string x, string y, int expected)
    {
        // Arrange
        var comparer = new VersionComparer();

        // Act
        var result = comparer.Compare(x, y);

        // Assert
        if (expected == 0)
        {
            result.Should().Be(0);
        }
        else if (expected > 0)
        {
            result.Should().BeGreaterThan(0);
        }
        else
        {
            result.Should().BeLessThan(0);
        }
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(null, "1.0.0", -1)]
    [InlineData("1.0.0", null, 1)]
    public void Compare_NullValues_ReturnsExpectedResult(string? x, string? y, int expected)
    {
        // Arrange
        var comparer = new VersionComparer();

        // Act
        var result = comparer.Compare(x, y);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("1.0.a")]
    [InlineData("1.0.0.0")]
    public void Compare_InvalidVersionFormat_ThrowsVersionFormatException(string invalidVersion)
    {
        // Arrange
        var comparer = new VersionComparer();

        // Act
        var actX = () => comparer.Compare(invalidVersion, "1.0.0");
        var actY = () => comparer.Compare("1.0.0", invalidVersion);

        // Assert
        actX.Should().Throw<VersionFormatException>();
        actY.Should().Throw<VersionFormatException>();
    }
}
