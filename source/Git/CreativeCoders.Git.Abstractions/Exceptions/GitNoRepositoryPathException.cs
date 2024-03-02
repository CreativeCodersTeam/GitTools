namespace CreativeCoders.Git.Abstractions.Exceptions;

public class GitNoRepositoryPathException(string path) : GitException($"'{path}' is not git repository");