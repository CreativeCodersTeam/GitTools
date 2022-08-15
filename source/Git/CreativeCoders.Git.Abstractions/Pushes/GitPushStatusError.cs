namespace CreativeCoders.Git.Abstractions.Pushes;

public class GitPushStatusError
{
    public GitPushStatusError(string reference, string message)
    {
        Reference = reference;
        Message = message;
    }

    public string Reference { get; }

    public string Message { get; }
}
