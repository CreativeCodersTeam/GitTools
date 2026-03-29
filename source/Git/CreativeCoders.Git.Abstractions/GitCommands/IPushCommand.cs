using System;
using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Pushes;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.GitCommands;

/// <summary>
/// Represents a fluent builder for configuring and executing a Git push operation.
/// </summary>
[PublicAPI]
public interface IPushCommand
{
    /// <summary>
    /// Enables automatic creation of the remote branch if it does not exist.
    /// </summary>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand CreateRemoteBranchIfNotExists();

    /// <summary>
    /// Configures whether the remote branch should be created if it does not exist.
    /// </summary>
    /// <param name="createRemoteBranchIfNotExists"><see langword="true"/> to create the remote branch if it does not exist; otherwise, <see langword="false"/>.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand CreateRemoteBranchIfNotExists(bool createRemoteBranchIfNotExists);

    /// <summary>
    /// Sets the branch to push.
    /// </summary>
    /// <param name="branch">The branch to push.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand Branch(IGitBranch branch);

    /// <summary>
    /// Enables user confirmation before the push is executed.
    /// </summary>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand Confirm();

    /// <summary>
    /// Configures whether user confirmation is required before the push is executed.
    /// </summary>
    /// <param name="confirm"><see langword="true"/> to require confirmation; otherwise, <see langword="false"/>.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand Confirm(bool confirm);

    /// <summary>
    /// Registers a handler to receive push status errors.
    /// </summary>
    /// <param name="pushStatusError">The handler to invoke when a push status error occurs.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnPushStatusError(Action<GitPushStatusError> pushStatusError);

    /// <summary>
    /// Registers a handler to receive pack builder progress updates.
    /// </summary>
    /// <param name="packBuilderProgress">The handler to invoke with pack builder progress information.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnPackBuilderProgress(Action<GitPackBuilderProgress> packBuilderProgress);

    /// <summary>
    /// Registers a handler to receive pack builder progress updates that can cancel the operation.
    /// </summary>
    /// <param name="packBuilderProgress">The handler to invoke with pack builder progress information. Returns <see langword="true"/> to continue; <see langword="false"/> to cancel.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnPackBuilderProgress(Func<GitPackBuilderProgress, bool> packBuilderProgress);

    /// <summary>
    /// Registers a handler to receive push transfer progress updates.
    /// </summary>
    /// <param name="transferProgress">The handler to invoke with transfer progress information.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnTransferProgress(Action<GitPushTransferProgress> transferProgress);

    /// <summary>
    /// Registers a handler to receive push transfer progress updates that can cancel the operation.
    /// </summary>
    /// <param name="transferProgress">The handler to invoke with transfer progress information. Returns <see langword="true"/> to continue; <see langword="false"/> to cancel.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnTransferProgress(Func<GitPushTransferProgress, bool> transferProgress);

    /// <summary>
    /// Registers a handler invoked after negotiation completes, before the push begins.
    /// </summary>
    /// <param name="negotiationCompletedBeforePush">The handler to invoke with the list of push updates.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnNegotiationCompletedBeforePush(Action<IEnumerable<GitPushUpdate>> negotiationCompletedBeforePush);

    /// <summary>
    /// Registers a handler invoked after negotiation completes that can cancel the push.
    /// </summary>
    /// <param name="negotiationCompletedBeforePush">The handler to invoke with the list of push updates. Returns <see langword="true"/> to continue; <see langword="false"/> to cancel.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnNegotiationCompletedBeforePush(Func<IEnumerable<GitPushUpdate>, bool> negotiationCompletedBeforePush);

    /// <summary>
    /// Registers a handler invoked with the list of commits that have not been pushed to the remote.
    /// </summary>
    /// <param name="unPushedCommits">The handler to invoke with the unpushed commits.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnUnPushedCommits(Action<IEnumerable<IGitCommit>> unPushedCommits);

    /// <summary>
    /// Registers a confirmation callback invoked before the push is executed.
    /// </summary>
    /// <param name="doConfirm">The callback that returns <see langword="true"/> to proceed with the push; <see langword="false"/> to cancel.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPushCommand OnConfirm(Func<bool> doConfirm);

    /// <summary>
    /// Executes the push operation.
    /// </summary>
    void Run();
}
