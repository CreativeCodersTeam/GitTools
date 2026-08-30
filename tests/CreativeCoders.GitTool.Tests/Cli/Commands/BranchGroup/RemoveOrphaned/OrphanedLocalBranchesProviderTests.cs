using AwesomeAssertions;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;
using FakeItEasy;
using Xunit;

namespace CreativeCoders.GitTool.Tests.Cli.Commands.BranchGroup.RemoveOrphaned;

public class OrphanedLocalBranchesProviderTests
{
    [Fact]
    public void GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var orphanedBranch = CreateBranch("refs/heads/feature/orphaned");

        var repository = CreateRepository(head, [head, orphanedBranch]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().ContainSingle().Which.Should().BeSameAs(orphanedBranch);
    }

    [Fact]
    public void GetOrphanedLocalBranches_LocalBranchWithRemoteCounterpart_DoesNotReturnBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var trackedBranch = CreateBranch("refs/heads/feature/tracked");

        var repository = CreateRepository(head, [head, trackedBranch],
            remoteBranchNames: ["feature/tracked"]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_CurrentHeadBranchWithoutRemoteCounterpart_DoesNotReturnBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/feature/current");

        var repository = CreateRepository(head, [head]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_RemoteBranchWithoutLocalCounterpart_DoesNotReturnBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var remoteBranch = CreateBranch("refs/remotes/origin/feature/remote-only", true);

        var repository = CreateRepository(head, [head, remoteBranch]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_AllLocalBranchesHaveRemoteCounterpart_ReturnsEmptyCollection()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var developBranch = CreateBranch("refs/heads/develop");
        var featureBranch = CreateBranch("refs/heads/feature/tracked");

        var repository = CreateRepository(head, [head, developBranch, featureBranch],
            remoteBranchNames: ["main", "develop", "feature/tracked"]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_MultipleOrphanedBranches_ReturnsAllOfThem()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var firstOrphanedBranch = CreateBranch("refs/heads/feature/first");
        var secondOrphanedBranch = CreateBranch("refs/heads/feature/second");
        var trackedBranch = CreateBranch("refs/heads/develop");

        var repository = CreateRepository(head,
            [head, firstOrphanedBranch, trackedBranch, secondOrphanedBranch],
            remoteBranchNames: ["main", "develop"]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEquivalentTo([firstOrphanedBranch, secondOrphanedBranch]);
    }

    [Fact]
    public void GetOrphanedLocalBranches_RemoteCounterpartDiffersInCase_DoesNotReturnBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var trackedBranch = CreateBranch("refs/heads/Feature/Tracked");

        var repository = CreateRepository(head, [head, trackedBranch],
            remoteBranchNames: ["feature/tracked"]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_CounterpartOnSecondRemote_DoesNotReturnBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var trackedBranch = CreateBranch("refs/heads/feature/tracked");
        var remoteBranch = CreateBranch("refs/remotes/upstream/feature/tracked", true);

        var repository = CreateRepository(head, [head, trackedBranch, remoteBranch],
            remoteNames: ["origin", "upstream"]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().BeEmpty();
    }

    [Fact]
    public void GetOrphanedLocalBranches_RemoteBranchNameEndsWithLocalName_ReturnsBranch()
    {
        // Arrange
        var head = CreateBranch("refs/heads/main");
        var orphanedBranch = CreateBranch("refs/heads/feature");

        // Only '<remote>/feature' is a counterpart, 'origin/other/feature' is a different branch
        var remoteBranch = CreateBranch("refs/remotes/origin/other/feature", true);

        var repository = CreateRepository(head, [head, orphanedBranch, remoteBranch]);

        var sut = new OrphanedLocalBranchesProvider(repository);

        // Act
        var orphanedBranches = sut.GetOrphanedLocalBranches();

        // Assert
        orphanedBranches.Should().ContainSingle().Which.Should().BeSameAs(orphanedBranch);
    }

    private static IGitBranch CreateBranch(string canonicalName, bool isRemote = false)
    {
        var branch = A.Fake<IGitBranch>();

        A.CallTo(() => branch.Name).Returns(new ReferenceName(canonicalName));
        A.CallTo(() => branch.IsRemote).Returns(isRemote);

        // GitBranch compares by its canonical name, the fake has to behave the same way
        A.CallTo(() => branch.Equals(A<IGitBranch>._))
            .ReturnsLazily((IGitBranch other) => other.Name.Canonical == canonicalName);

        return branch;
    }

    private static IGitRepository CreateRepository(IGitBranch head, IGitBranch[] branches,
        string[]? remoteBranchNames = null, string[]? remoteNames = null)
    {
        var remotes = remoteNames ?? ["origin"];

        var allBranches = branches
            .Concat((remoteBranchNames ?? [])
                .Select(x => CreateBranch($"refs/remotes/{remotes[0]}/{x}", true)))
            .ToArray();

        var branchCollection = A.Fake<IGitBranchCollection>();

        A.CallTo(() => branchCollection.GetEnumerator())
            .ReturnsLazily(() => ((IEnumerable<IGitBranch>)allBranches).GetEnumerator());

        var repository = A.Fake<IGitRepository>();

        A.CallTo(() => repository.Head).Returns(head);
        A.CallTo(() => repository.Branches).Returns(branchCollection);
        A.CallTo(() => repository.Remotes).Returns(CreateRemotes(remotes));

        return repository;
    }

    private static IGitRemoteCollection CreateRemotes(string[] remoteNames)
    {
        var remotes = remoteNames
            .Select(remoteName =>
            {
                var remote = A.Fake<IGitRemote>();

                A.CallTo(() => remote.Name).Returns(remoteName);

                return remote;
            })
            .ToArray();

        var remoteCollection = A.Fake<IGitRemoteCollection>();

        A.CallTo(() => remoteCollection.GetEnumerator())
            .ReturnsLazily(() => ((IEnumerable<IGitRemote>)remotes).GetEnumerator());

        return remoteCollection;
    }
}
