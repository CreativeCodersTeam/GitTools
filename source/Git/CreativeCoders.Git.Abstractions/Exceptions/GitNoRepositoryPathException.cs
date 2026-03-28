namespace CreativeCoders.Git.Abstractions.Exceptions;

/// <summary>
/// Represents the exception thrown when a specified path is not a valid Git repository.
/// </summary>
/// <param name="path">The path that is not a Git repository.</param>
public class GitNoRepositoryPathException(string path) : GitException($"'{path}' is not git repository");