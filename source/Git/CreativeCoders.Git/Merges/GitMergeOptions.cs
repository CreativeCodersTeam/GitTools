namespace CreativeCoders.Git.Merges;

public class GitMergeOptions
{
    public GitMergeOptions()
    {
        
    }

    internal MergeOptions ToMergeOptions()
    {
        return new MergeOptions();
    }
}
