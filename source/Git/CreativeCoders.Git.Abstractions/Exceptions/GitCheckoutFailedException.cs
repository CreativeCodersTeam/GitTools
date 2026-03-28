using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a Git checkout operation fails.
/// </summary>
[PublicAPI]
public class GitCheckoutFailedException : GitException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitCheckoutFailedException"/> class.
    /// </summary>
    public GitCheckoutFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitCheckoutFailedException"/> class with the name of the branch that could not be checked out.
    /// </summary>
    /// <param name="branchName">The name of the branch that failed to check out.</param>
    public GitCheckoutFailedException(string? branchName) : base($"Checkout branch '{branchName}' failed")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitCheckoutFailedException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GitCheckoutFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}