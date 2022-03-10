namespace CreativeCoders.GitTool.Base;

public class GitPullRequest
{
    public GitPullRequest(string url)
    {
        Url = url;
    }

    public string Url { get; }
}