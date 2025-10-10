using CreativeCoders.Git.Abstractions.Branches;
using AwesomeAssertions;
using Xunit;

namespace CreativeCoders.Git.UnitTests.Branches;

public class GitBranchesTests
{
    [Fact]
    public void LocalGetCanonicalName_ForMaster_ReturnsRefsHeadMaster()
    {
        // Act
        var branchName = GitBranchNames.Local.GetCanonicalName(GitMainBranch.Master);

        // Assert
        branchName
            .Should()
            .Be("refs/heads/master");
    }

    [Fact]
    public void LocalGetFriendlyName_ForMaster_ReturnsMaster()
    {
        // Act
        var branchName = GitBranchNames.Local.GetFriendlyName(GitMainBranch.Master);

        // Assert
        branchName
            .Should()
            .Be("master");
    }

    [Fact]
    public void LocalGetCanonicalName_ForMain_ReturnsRefsHeadMain()
    {
        // Act
        var branchName = GitBranchNames.Local.GetCanonicalName(GitMainBranch.Main);

        // Assert
        branchName
            .Should()
            .Be("refs/heads/main");
    }

    [Fact]
    public void LocalGetFriendlyName_ForMain_ReturnsMain()
    {
        // Act
        var branchName = GitBranchNames.Local.GetFriendlyName(GitMainBranch.Main);

        // Assert
        branchName
            .Should()
            .Be("main");
    }

    [Fact]
    public void LocalGetCanonicalName_ForCustom_ReturnsEmptyString()
    {
        // Act
        var branchName = GitBranchNames.Local.GetCanonicalName(GitMainBranch.Custom);

        // Assert
        branchName
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void LocalGetFriendlyName_ForCustom_ReturnsEmptyString()
    {
        // Act
        var branchName = GitBranchNames.Local.GetFriendlyName(GitMainBranch.Custom);

        // Assert
        branchName
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void RemoteGetCanonicalName_ForMaster_ReturnsRefsRemoteOriginMaster()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetCanonicalName(GitMainBranch.Master);

        // Assert
        branchName
            .Should()
            .Be("refs/remotes/origin/master");
    }

    [Fact]
    public void RemoteGetFriendlyName_ForMaster_ReturnsOriginMaster()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetFriendlyName(GitMainBranch.Master);

        // Assert
        branchName
            .Should()
            .Be("origin/master");
    }

    [Fact]
    public void RemoteGetCanonicalName_ForMain_ReturnsRefsRemoteOriginMain()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetCanonicalName(GitMainBranch.Main);

        // Assert
        branchName
            .Should()
            .Be("refs/remotes/origin/main");
    }

    [Fact]
    public void RemoteGetFriendlyName_ForMain_ReturnsOriginMain()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetFriendlyName(GitMainBranch.Main);

        // Assert
        branchName
            .Should()
            .Be("origin/main");
    }

    [Fact]
    public void RemoteGetCanonicalName_ForCustom_ReturnsEmptyString()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetCanonicalName(GitMainBranch.Custom);

        // Assert
        branchName
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void RemoteGetFriendlyName_ForCustom_ReturnsEmptyString()
    {
        // Act
        var branchName = GitBranchNames.Remote.GetFriendlyName(GitMainBranch.Custom);

        // Assert
        branchName
            .Should()
            .BeEmpty();
    }
}