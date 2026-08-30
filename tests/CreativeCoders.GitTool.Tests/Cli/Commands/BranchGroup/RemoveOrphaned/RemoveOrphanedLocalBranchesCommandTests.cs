using AwesomeAssertions;
using CreativeCoders.Cli.Hosting.Exceptions;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Exceptions;
using CreativeCoders.GitTool.Base;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;
using FakeItEasy;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace CreativeCoders.GitTool.Tests.Cli.Commands.BranchGroup.RemoveOrphaned;

public class RemoveOrphanedLocalBranchesCommandTests
{
    [Fact]
    public async Task ExecuteAsync_AllOptionSet_DeletesAllOrphanedBranchesWithoutSelection()
    {
        // Arrange
        var firstBranch = CreateBranch("refs/heads/feature/first");
        var secondBranch = CreateBranch("refs/heads/feature/second");

        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        // A non interactive console makes the test fail if the command tries to show the selection prompt
        var sut = CreateSut(new TestConsole(), gitRepository, firstBranch, secondBranch);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { All = true });

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/first")).MustHaveHappenedOnceExactly();
        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/second")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecuteAsync_BranchDeselectedInPrompt_DeletesOnlySelectedBranches()
    {
        // Arrange
        var firstBranch = CreateBranch("refs/heads/feature/first");
        var secondBranch = CreateBranch("refs/heads/feature/second");

        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        var console = new TestConsole().Interactive();

        // Move to the second branch, deselect it and accept the selection
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);

        var sut = CreateSut(console, gitRepository, firstBranch, secondBranch);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/first")).MustHaveHappenedOnceExactly();
        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/second")).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_AllBranchesDeselectedInPrompt_DeletesNoBranch()
    {
        // Arrange
        var firstBranch = CreateBranch("refs/heads/feature/first");
        var secondBranch = CreateBranch("refs/heads/feature/second");

        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        var console = new TestConsole().Interactive();

        // Deselect both branches and accept the empty selection
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);

        var sut = CreateSut(console, gitRepository, firstBranch, secondBranch);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        A.CallTo(() => branchCollection.DeleteLocalBranch(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_NoOrphanedBranchesFound_DeletesNoBranch()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        var sut = CreateSut(new TestConsole(), gitRepository);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        A.CallTo(() => branchCollection.DeleteLocalBranch(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_DeletingBranchFails_DeletesRemainingBranchesAndReturnsErrorCode()
    {
        // Arrange
        var firstBranch = CreateBranch("refs/heads/feature/first");
        var secondBranch = CreateBranch("refs/heads/feature/second");

        var branchCollection = A.Fake<IGitBranchCollection>();

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/first"))
            .Throws(new GitException("Branch is locked"));

        var gitRepository = CreateRepository(branchCollection);

        var sut = CreateSut(new TestConsole(), gitRepository, firstBranch, secondBranch);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { All = true });

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.DeleteLocalBranchFailed);

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/second")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecuteAsync_SkipFetchPruneNotSet_FetchesPruneFromOrigin()
    {
        // Arrange
        var gitRepository = CreateRepository(A.Fake<IGitBranchCollection>());

        var sut = CreateSut(new TestConsole(), gitRepository);

        // Act
        await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        A.CallTo(() =>
                gitRepository.Fetch(GitRemotes.Origin, A<GitFetchOptions>.That.Matches(x => x.Prune == true)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecuteAsync_SkipFetchPruneSet_DoesNotFetchFromRemote()
    {
        // Arrange
        var gitRepository = CreateRepository(A.Fake<IGitBranchCollection>());

        var sut = CreateSut(new TestConsole(), gitRepository);

        // Act
        await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { SkipFetchPrune = true });

        // Assert
        A.CallTo(() => gitRepository.Fetch(A<string>._, A<GitFetchOptions>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_FetchPruneFails_WritesWarningAndShowsSelection()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        A.CallTo(() => gitRepository.Fetch(A<string>._, A<GitFetchOptions>._))
            .Throws(new GitException("Remote not reachable"));

        var console = new TestConsole().Interactive();

        // Accept the selection as it is offered
        console.Input.PushKey(ConsoleKey.Enter);

        var sut = CreateSut(console, gitRepository, CreateBranch("refs/heads/feature/first"));

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        console.Output.Should().Contain("Fetch prune failed").And.Contain("Remote not reachable");

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/first")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecuteAsync_FetchPruneFailsWithAllOption_ShowsSelectionInsteadOfDeletingAll()
    {
        // Arrange
        var firstBranch = CreateBranch("refs/heads/feature/first");
        var secondBranch = CreateBranch("refs/heads/feature/second");

        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        A.CallTo(() => gitRepository.Fetch(A<string>._, A<GitFetchOptions>._))
            .Throws(new GitException("Remote not reachable"));

        var console = new TestConsole().Interactive();

        // Move to the second branch, deselect it and accept the selection
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);

        var sut = CreateSut(console, gitRepository, firstBranch, secondBranch);

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { All = true });

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/first")).MustHaveHappenedOnceExactly();
        A.CallTo(() => branchCollection.DeleteLocalBranch("feature/second")).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_FetchPruneFailsWithAllOptionOnNonInteractiveTerminal_AbortsWithoutDeleting()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        A.CallTo(() => gitRepository.Fetch(A<string>._, A<GitFetchOptions>._))
            .Throws(new GitException("Remote not reachable"));

        var sut = CreateSut(new TestConsole(), gitRepository, CreateBranch("refs/heads/feature/first"));

        // Act
        var execute = async () =>
            await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { All = true });

        // Assert
        var abortException = await execute.Should().ThrowAsync<CliCommandAbortException>();

        abortException.Which.ExitCode.Should().Be(ReturnCodes.NoInteractiveTerminal);
        abortException.Which.Message.Should().Contain("stale remote state").And.Contain("'--skip-fetch-prune'");

        A.CallTo(() => branchCollection.DeleteLocalBranch(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_BranchNameContainsMarkupCharacters_WritesUnescapedBranchName()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        var console = new TestConsole();

        var sut = CreateSut(console, gitRepository, CreateBranch("refs/heads/feature/[wip]-thing"));

        // Act
        var result = await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions { All = true });

        // Assert
        result.ExitCode.Should().Be(ReturnCodes.Success);

        console.Output.Should().Contain("feature/[wip]-thing");
    }

    [Fact]
    public async Task ExecuteAsync_NonInteractiveTerminalWithoutAllOption_AbortsWithoutDeletingBranches()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        var sut = CreateSut(new TestConsole(), gitRepository, CreateBranch("refs/heads/feature/first"));

        // Act
        var execute = async () => await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        var abortException = await execute.Should().ThrowAsync<CliCommandAbortException>();

        abortException.Which.ExitCode.Should().Be(ReturnCodes.NoInteractiveTerminal);
        abortException.Which.Message.Should().Contain("'--all'").And.NotContain("'--skip-fetch-prune'");

        A.CallTo(() => branchCollection.DeleteLocalBranch(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task
        ExecuteAsync_FetchPruneFailsOnNonInteractiveTerminalWithoutAllOption_AbortMessageNamesBothOptions()
    {
        // Arrange
        var branchCollection = A.Fake<IGitBranchCollection>();
        var gitRepository = CreateRepository(branchCollection);

        A.CallTo(() => gitRepository.Fetch(A<string>._, A<GitFetchOptions>._))
            .Throws(new GitException("Remote not reachable"));

        var sut = CreateSut(new TestConsole(), gitRepository, CreateBranch("refs/heads/feature/first"));

        // Act
        var execute = async () => await sut.ExecuteAsync(new RemoveOrphanedLocalBranchesOptions());

        // Assert
        var abortException = await execute.Should().ThrowAsync<CliCommandAbortException>();

        abortException.Which.ExitCode.Should().Be(ReturnCodes.NoInteractiveTerminal);
        abortException.Which.Message.Should().Contain("'--all'").And.Contain("'--skip-fetch-prune'");

        A.CallTo(() => branchCollection.DeleteLocalBranch(A<string>._)).MustNotHaveHappened();
    }

    private static RemoveOrphanedLocalBranchesCommand CreateSut(IAnsiConsole ansiConsole,
        IGitRepository gitRepository, params IGitBranch[] orphanedBranches)
    {
        var orphanedLocalBranchesProvider = A.Fake<IOrphanedLocalBranchesProvider>();

        A.CallTo(() => orphanedLocalBranchesProvider.GetOrphanedLocalBranches()).Returns(orphanedBranches);

        return new RemoveOrphanedLocalBranchesCommand(ansiConsole, A.Fake<ICml>(), gitRepository,
            orphanedLocalBranchesProvider);
    }

    private static IGitRepository CreateRepository(IGitBranchCollection branchCollection)
    {
        var gitRepository = A.Fake<IGitRepository>();

        A.CallTo(() => gitRepository.Branches).Returns(branchCollection);

        return gitRepository;
    }

    private static IGitBranch CreateBranch(string canonicalName)
    {
        var branch = A.Fake<IGitBranch>();

        A.CallTo(() => branch.Name).Returns(new ReferenceName(canonicalName));

        return branch;
    }
}
