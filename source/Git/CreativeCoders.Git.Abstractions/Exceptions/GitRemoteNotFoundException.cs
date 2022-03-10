namespace CreativeCoders.Git.Abstractions.Exceptions;

public class GitRemoteNotFoundException : GitException
{
    public GitRemoteNotFoundException(string remoteName)
        : base($"Remote '{remoteName}' not found")
    {
    }
}