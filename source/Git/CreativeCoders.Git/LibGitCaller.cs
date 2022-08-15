using CreativeCoders.Git.Abstractions.Exceptions;

namespace CreativeCoders.Git;

internal class LibGitCaller : ILibGitCaller
{
    public void Invoke(Action action)
    {
        try
        {
            action();
        }
        catch (LibGit2SharpException e)
        {
            throw new GitException(e.Message, e);
        }
    }

    public T Invoke<T>(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (LibGit2SharpException e)
        {
            throw new GitException(e.Message, e);
        }
    }
}
