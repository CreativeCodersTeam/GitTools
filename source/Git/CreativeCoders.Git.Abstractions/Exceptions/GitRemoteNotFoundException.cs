namespace CreativeCoders.Git.Abstractions.Exceptions;

public class GitRemoteNotFoundException(string remoteName) : GitException($"Remote '{remoteName}' not found");