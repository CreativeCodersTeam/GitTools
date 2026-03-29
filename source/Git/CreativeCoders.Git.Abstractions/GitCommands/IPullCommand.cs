using System;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Fetches;
using CreativeCoders.Git.Abstractions.Merges;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.GitCommands;

/// <summary>
/// Represents a fluent builder for configuring and executing a Git pull operation.
/// </summary>
[PublicAPI]
public interface IPullCommand
{
    /// <summary>
    /// Registers a simple checkout notification handler for all notification types.
    /// </summary>
    /// <param name="notify">The handler to invoke on checkout notifications.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify);

    /// <summary>
    /// Registers a simple checkout notification handler for the specified notification types.
    /// </summary>
    /// <param name="notify">The handler to invoke on checkout notifications.</param>
    /// <param name="notifyFlags">A bitwise combination of the enumeration values that specifies which notifications to receive.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnCheckoutNotify(GitSimpleCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    /// <summary>
    /// Registers a checkout notification handler that can cancel the operation, for all notification types.
    /// </summary>
    /// <param name="notify">The handler to invoke on checkout notifications.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify);

    /// <summary>
    /// Registers a checkout notification handler that can cancel the operation, for the specified notification types.
    /// </summary>
    /// <param name="notify">The handler to invoke on checkout notifications.</param>
    /// <param name="notifyFlags">A bitwise combination of the enumeration values that specifies which notifications to receive.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnCheckoutNotify(GitCheckoutNotifyHandler notify, GitCheckoutNotifyFlags notifyFlags);

    /// <summary>
    /// Registers a handler to receive checkout progress updates.
    /// </summary>
    /// <param name="progress">The handler to invoke with checkout progress information.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnCheckoutProgress(GitCheckoutProgressHandler progress);

    /// <summary>
    /// Registers a handler to receive fetch transfer progress updates.
    /// </summary>
    /// <param name="progress">The handler to invoke with transfer progress information.</param>
    /// <returns>The current command instance for method chaining.</returns>
    IPullCommand OnTransferProgress(Action<GitFetchTransferProgress> progress);

    /// <summary>
    /// Executes the pull operation.
    /// </summary>
    /// <returns>The result of the merge operation performed during the pull.</returns>
    GitMergeResult Run();
}
