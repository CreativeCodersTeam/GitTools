using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the base exception for all Git-related errors.
/// </summary>
[ExcludeFromCodeCoverage]
[PublicAPI]
public class GitException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitException"/> class.
    /// </summary>
    public GitException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GitException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GitException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}