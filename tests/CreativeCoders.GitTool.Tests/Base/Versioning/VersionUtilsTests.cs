using AwesomeAssertions;
using CreativeCoders.GitTool.Base.Versioning;
using Xunit;

namespace CreativeCoders.GitTool.Tests.Base.Versioning;

public class VersionUtilsTests
{
    [Theory]
    [InlineData("1.2.3", true, "1.2.3")]
    [InlineData("v1.2.3", true, "1.2.3")]
    [InlineData("version1.0", true, "1.0")]
    [InlineData("1.2", true, "1.2")]
    [InlineData("V1.2", true, "1.2")]
    [InlineData("VERSION1.2", true, "1.2")]
    [InlineData("1.2.3", false, "1.2.3")]
    public void IsValidVersion_ValidVersions_ReturnsTrueAndNormalizedVersion(string version, bool ignorePrefix,
        string expectedNormalized)
    {
        // Act
        var result = VersionUtils.IsValidVersion(version, out var normalizedVersion, ignorePrefix);

        // Assert
        result.Should().BeTrue();
        normalizedVersion.Should().Be(expectedNormalized);
    }

    [Theory]
    [InlineData("1.a.3", true)]
    [InlineData("v1.2.x", true)]
    [InlineData("1..2", true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("v1.2.3", false)]
    [InlineData("version1.0", false)]
    public void IsValidVersion_InvalidVersions_ReturnsFalseAndEmptyNormalizedVersion(string version, bool ignorePrefix)
    {
        // Act
        var result = VersionUtils.IsValidVersion(version, out var normalizedVersion, ignorePrefix);

        // Assert
        result.Should().BeFalse();
        normalizedVersion.Should().BeEmpty();
    }

    [Theory]
    [InlineData("v1.2.3", "1.2.3")]
    [InlineData("version1.0", "1.0")]
    [InlineData("V2.0", "2.0")]
    [InlineData("VERSION3.1", "3.1")]
    [InlineData("1.2.3", "1.2.3")]
    [InlineData("v", "")]
    [InlineData("version", "")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void RemoveLeadingVersionPrefix_VariousInputs_ReturnsExpectedResult(string version, string expected)
    {
        // Act
        var result = VersionUtils.RemoveLeadingVersionPrefix(version);

        // Assert
        result.Should().Be(expected);
    }
}
