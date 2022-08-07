namespace CreativeCoders.Git;

public interface ILibGitCaller
{
    void Invoke(Action action);
    T Invoke<T>(Func<T> func);
}
