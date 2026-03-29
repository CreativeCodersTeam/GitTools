using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a Git push operation fails.
/// </summary>
[PublicAPI]
public class GitPushFailedException : GitException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushFailedException"/> class.
    /// </summary>
    public GitPushFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushFailedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GitPushFailedException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitPushFailedException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GitPushFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}