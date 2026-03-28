namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when no remote is configured for the repository.
/// </summary>
public class GitNoRemoteFoundException() : GitException("No remote can be found");