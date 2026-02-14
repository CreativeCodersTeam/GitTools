using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using CreativeCoders.GitTool.Base.Versioning;
using Xunit;

namespace CreativeCoders.GitTool.Tests.Base.Versioning;

public class VersionBuilderTests
{
    [Theory]
    [InlineData("1.2.3", "1.2.3")]
    [InlineData("1.2", "1.2.0")]
    [InlineData("1", "1.0.0")]
    public void Build_ValidVersion_ReturnsCorrectVersionString(string version, string expected)
    {
        // Arrange
        var builder = new VersionBuilder(version);

        // Act
        var result = builder.Build();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(" 1.2.3 ", VersionFormatKind.Loose, "1.2.3")]
    [InlineData("1 . 2 . 3", VersionFormatKind.Loose, "1.2.3")]
    public void Build_VersionWithWhitespaceInLooseMode_ReturnsCleanedVersion(string version,
        VersionFormatKind formatKind, string expected)
    {
        // Arrange
        var builder = new VersionBuilder(version, formatKind);

        // Act
        var result = builder.Build();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(" 1.2.3")]
    [InlineData("1.2.3 ")]
    [InlineData("1. 2.3")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public void Constructor_VersionWithWhitespaceInStrictMode_ThrowsVersionFormatException(string version)
    {
        // Act
        Action act = () => _ = new VersionBuilder(version, VersionFormatKind.Strict);

        // Assert
        act.Should().Throw<VersionFormatException>()
            .And.Version.Should().Be(version);
    }

    [Theory]
    [InlineData("1.2.3.4")]
    [InlineData("a.b.c")]
    [InlineData("1.2.c")]
    public void Constructor_InvalidVersionFormat_ThrowsVersionFormatException(string version)
    {
        // Act
        Action act = () => _ = new VersionBuilder(version);

        // Assert
        act.Should().Throw<VersionFormatException>()
            .And.Version.Should().Be(version);
    }

    [Fact]
    public void IncrementPatch_DefaultIncrement_IncrementsByOne()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementPatch();
        var result = builder.Build();

        // Assert
        result.Should().Be("1.2.4");
    }

    [Fact]
    public void IncrementPatch_CustomIncrement_IncrementsByValue()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementPatch(5);
        var result = builder.Build();

        // Assert
        result.Should().Be("1.2.8");
    }

    [Fact]
    public void IncrementMinor_DefaultIncrement_IncrementsByOne()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMinor();
        var result = builder.Build();

        // Assert
        result.Should().Be("1.3.0");
    }

    [Fact]
    public void IncrementMinor_WithOutPatchReset_IncrementsByOne()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMinor(1, false);
        var result = builder.Build();

        // Assert
        result.Should().Be("1.3.3");
    }

    [Fact]
    public void IncrementMinor_CustomIncrement_IncrementsByValue()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMinor(10);
        var result = builder.Build();

        // Assert
        result.Should().Be("1.12.0");
    }

    [Fact]
    public void IncrementMajor_DefaultIncrement_IncrementsByOne()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMajor();
        var result = builder.Build();

        // Assert
        result.Should().Be("2.0.0");
    }

    [Fact]
    public void IncrementMajor_WithOutResetMinorAndPatch_MinorAndPatchAreResetToZero()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMajor(1, false);
        var result = builder.Build();

        // Assert
        result.Should().Be("2.2.3");
    }

    [Fact]
    public void IncrementMajor_CustomIncrement_IncrementsByValue()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        builder.IncrementMajor(2);
        var result = builder.Build();

        // Assert
        result.Should().Be("3.0.0");
    }

    [Fact]
    public void MultipleIncrements_WithOutResetLowerVersionParts_ReturnsCorrectFinalVersion()
    {
        // Arrange
        var builder = new VersionBuilder("1.1.1");

        // Act
        builder.IncrementMajor(1, false);
        builder.IncrementMinor(2, false);
        builder.IncrementPatch(3);
        var result = builder.Build();

        // Assert
        result.Should().Be("2.3.4");
    }

    [Fact]
    public void MajorMinorPatch_Get_ReturnCorrectVersionParts()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        var major = builder.Major;
        var minor = builder.Minor;
        var patch = builder.Patch;

        // Assert
        major.Should().Be(1);
        minor.Should().Be(2);
        patch.Should().Be(3);
    }

    [Fact]
    public void GetVersionPart_Get_ReturnCorrectVersionParts()
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        var major = builder.GetVersionPart(VersionBuilder.MajorPartIndex);
        var minor = builder.GetVersionPart(VersionBuilder.MinorPartIndex);
        var patch = builder.GetVersionPart(VersionBuilder.PatchPartIndex);

        // Assert
        major.Should().Be(1);
        minor.Should().Be(2);
        patch.Should().Be(3);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void GetVersionPart_InvalidIndex_ThrowsArgumentOutOfRangeException(int partIndex)
    {
        // Arrange
        var builder = new VersionBuilder("1.2.3");

        // Act
        Action act = () => _ = builder.GetVersionPart(partIndex);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
