namespace CreativeCoders.Git;

/// <summary>
/// Provides a wrapper for invoking LibGit2Sharp operations with unified exception handling.
/// </summary>
public interface ILibGitCaller
{
    /// <summary>
    /// Invokes the specified action, translating LibGit2Sharp exceptions into <see cref="Abstractions.Exceptions.GitException"/>.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    void Invoke(Action action);

    /// <summary>
    /// Invokes the specified function, translating LibGit2Sharp exceptions into <see cref="Abstractions.Exceptions.GitException"/>.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <returns>The result of the function invocation.</returns>
    T Invoke<T>(Func<T> func);
}
