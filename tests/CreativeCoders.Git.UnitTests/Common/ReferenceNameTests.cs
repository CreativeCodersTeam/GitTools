using System;
using CreativeCoders.Git.Abstractions.Common;
using FluentAssertions;
using Xunit;

namespace CreativeCoders.Git.UnitTests.Common
{
    public class ReferenceNameTests
    {
        [Theory]
        [InlineData("main")]
        [InlineData("develop")]
        [InlineData("test1234")]
        [InlineData("feature/new-feature")]
        public void Ctor_LocalBranch_PropertiesAreCorrect(string branchName)
        {
            const string prefix = "refs/heads/";

            // Act
            var referenceName = new ReferenceName(prefix + branchName);

            // Assert
            referenceName.Canonical
                .Should()
                .Be(prefix + branchName);

            referenceName.Friendly
                .Should()
                .Be(branchName);

            referenceName.WithoutRemote
                .Should()
                .Be(branchName);

            referenceName.IsLocalBranch
                .Should()
                .BeTrue();

            referenceName.IsPullRequest
                .Should()
                .BeFalse();

            referenceName.IsRemoteBranch
                .Should()
                .BeFalse();

            referenceName.IsTag
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("main")]
        [InlineData("develop")]
        [InlineData("test1234")]
        [InlineData("feature/new-feature")]
        public void Ctor_RemoteBranch_PropertiesAreCorrect(string branchName)
        {
            const string origin = "origin";
            const string prefix = "refs/remotes/" + origin + "/";

            // Act
            var referenceName = new ReferenceName(prefix + branchName);

            // Assert
            referenceName.Canonical
                .Should()
                .Be(prefix + branchName);

            referenceName.Friendly
                .Should()
                .Be(origin + "/" + branchName);

            referenceName.WithoutRemote
                .Should()
                .Be(branchName);

            referenceName.IsLocalBranch
                .Should()
                .BeFalse();

            referenceName.IsPullRequest
                .Should()
                .BeFalse();

            referenceName.IsRemoteBranch
                .Should()
                .BeTrue();

            referenceName.IsTag
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("refs/heads/main")]
        [InlineData("refs/heads/feature/test-feature")]
        [InlineData("refs/remotes/origin/main")]
        [InlineData("refs/remotes/origin/feature/test-feature")]
        public void TryParse_CorrectCanonicalName_ReturnsTrueAndReferenceName(string canonicalBranchName)
        {
            // Act
            var result = ReferenceName.TryParse(canonicalBranchName, out var referenceName);

            // Assert
            result
                .Should()
                .BeTrue();

            referenceName
                .Should()
                .NotBeNull()
                .And
                .Match<ReferenceName>(x => x.Canonical == canonicalBranchName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("main")]
        [InlineData("heads/feature/test-feature")]
        [InlineData("refs/origin/main")]
        [InlineData("origin/feature/test-feature")]
        public void TryParse_IncorrectCanonicalName_ReturnsFalse(string canonicalBranchName)
        {
            // Act
            var result = ReferenceName.TryParse(canonicalBranchName, out var referenceName);

            // Assert
            result
                .Should()
                .BeFalse();

            referenceName
                .Should()
                .BeNull();
        }

        [Theory]
        [InlineData("refs/heads/main")]
        [InlineData("refs/heads/feature/test-feature")]
        [InlineData("refs/remotes/origin/main")]
        [InlineData("refs/remotes/origin/feature/test-feature")]
        public void Parse_CorrectCanonicalName_ReturnsReferenceName(string canonicalBranchName)
        {
            // Act
            var referenceName = ReferenceName.Parse(canonicalBranchName);

            // Assert
            referenceName
                .Should()
                .NotBeNull()
                .And
                .Match<ReferenceName>(x => x.Canonical == canonicalBranchName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("main")]
        [InlineData("heads/feature/test-feature")]
        [InlineData("refs/origin/main")]
        [InlineData("origin/feature/test-feature")]
        public void Parse_IncorrectCanonicalName_ThrowsException(string canonicalBranchName)
        {
            // Act
            Action act = () => ReferenceName.Parse(canonicalBranchName);

            // Assert
            act
                .Should()
                .Throw<ArgumentException>();
        }
    }
}
