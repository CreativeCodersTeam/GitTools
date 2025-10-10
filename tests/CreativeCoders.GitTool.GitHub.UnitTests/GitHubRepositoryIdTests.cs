using System;
using AwesomeAssertions;
using Xunit;

namespace CreativeCoders.GitTool.GitHub.UnitTests;

public class GitHubRepositoryIdTests
{
    [Theory]
    [InlineData("the_owner/repo1.git", "the_owner", "repo1")]
    [InlineData("the_owner/repo1", "the_owner", "repo1")]
    public void Ctor_DifferentRepositoryPathFormats_SameOwnerAndRepositoryName(
        string repositoryPath, string owner, string repositoryName)
    {
        var repositoryUrl = new Uri($"https://github.com/{repositoryPath}");
        // Act
        var repositoryId = new GitHubRepositoryId(repositoryUrl);

        // Assert
        repositoryId.Owner
            .Should()
            .Be(owner);

        repositoryId.RepositoryName
            .Should()
            .Be(repositoryName);
    }

    [Theory]
    [InlineData("https://github.com/test")]
    [InlineData("https://github.com/test/1234/abcd")]
    public void Ctor_MismatchingRepositoryUrls_ThrowsException(string repositoryUrl)
    {
        // Act
        var act = () => new GitHubRepositoryId(new Uri(repositoryUrl));

        // Assert
        act
            .Should()
            .Throw<ArgumentException>();
    }
}
