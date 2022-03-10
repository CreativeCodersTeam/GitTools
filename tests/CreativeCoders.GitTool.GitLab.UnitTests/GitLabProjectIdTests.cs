using System;
using FluentAssertions;
using GitLabApiClient.Internal.Paths;
using Xunit;

namespace CreativeCoders.GitTool.GitLab.UnitTests;

public class GitLabProjectIdTests
{
    [Theory]
    [InlineData("the_owner/repo1.git", "the_owner", "repo1")]
    [InlineData("the_owner/repo1", "the_owner", "repo1")]
    [InlineData("the_owner/sub/repo1.git", "the_owner", "sub", "repo1")]
    [InlineData("the_owner/sub/repo1", "the_owner", "sub", "repo1")]
    public void Ctor_DifferentRepositoryPathFormats_SameOwnerAndRepositoryName(
        string repositoryPath, params string[] parts)
    {
        var repositoryUrl = new Uri($"https://gitlab.com/{repositoryPath}");
        // Act
        var projectId = GitLabProjectId.GetProjectId(repositoryUrl);

        // Assert
        projectId.ToString()
            .Should()
            .Be(Uri.EscapeDataString(string.Join('/', parts)));
    }

    [Theory]
    [InlineData("https://github.com/test")]
    [InlineData("https://github.com/")]
    public void Ctor_MismatchingRepositoryUrls_ThrowsException(string repositoryUrl)
    {
        // Act
        Func<ProjectId> act = () => GitLabProjectId.GetProjectId(new Uri(repositoryUrl));

        // Assert
        act
            .Should()
            .Throw<ArgumentException>();
    }
}