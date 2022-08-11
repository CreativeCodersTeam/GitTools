namespace CreativeCoders.Git.Abstractions.Pushes;

public class GitPackBuilderProgress
{
    public GitPackBuilderProgress(GitPackBuilderStage stage, int current, int total)
    {
        Stage = stage;
        Current = current;
        Total = total;
    }

    public GitPackBuilderStage Stage { get; }

    public int Current { get; }

    public int Total { get; }
}
