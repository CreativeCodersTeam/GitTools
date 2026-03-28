namespace CreativeCoders.Git.Merges;

/// <summary>
/// Represents configuration options for a Git merge operation.
/// </summary>
public class GitMergeOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitMergeOptions"/> class.
    /// </summary>
    public GitMergeOptions()
    {

    }

    internal MergeOptions ToMergeOptions()
    {
        return new MergeOptions();
    }
}
