using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a specified Git branch does not exist.
/// </summary>
[PublicAPI]
[ExcludeFromCodeCoverage]
public class GitBranchNotExistsException : GitException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitBranchNotExistsException"/> class.
    /// </summary>
    public GitBranchNotExistsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitBranchNotExistsException"/> class with the name of the branch that was not found.
    /// </summary>
    /// <param name="branchName">The name of the branch that does not exist.</param>
    public GitBranchNotExistsException(string? branchName) : base($"Branch '{branchName}' does not exist")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitBranchNotExistsException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GitBranchNotExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}