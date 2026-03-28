using CreativeCoders.Git.Abstractions.Exceptions;

namespace CreativeCoders.Git;

/// <summary>
/// Wraps LibGit2Sharp calls and translates <see cref="LibGit2SharpException"/> into <see cref="GitException"/>.
/// </summary>
internal class LibGitCaller : ILibGitCaller
{
    /// <inheritdoc />
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

    /// <inheritdoc />
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
