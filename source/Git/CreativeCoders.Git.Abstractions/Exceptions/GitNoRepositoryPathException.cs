namespace CreativeCoders.Git.Abstractions.Exceptions;

public class GitNoRepositoryPathException : GitException
{
    public GitNoRepositoryPathException(string path) : base($"'{path}' is not git repository")
    {
            
    }
}