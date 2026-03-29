using System;

namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a Git operation cannot proceed because a file is locked.
/// </summary>
/// <param name="innerException">The exception that is the cause of the current exception.</param>
public class GitLockedFileException(Exception innerException) : GitException("A file is locked", innerException);