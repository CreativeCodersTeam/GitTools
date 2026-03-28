namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a specified Git remote is not found.
/// </summary>
/// <param name="remoteName">The name of the remote that was not found.</param>
public class GitRemoteNotFoundException(string remoteName) : GitException($"Remote '{remoteName}' not found");