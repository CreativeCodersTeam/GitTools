using System.Diagnostics.CodeAnalysis;

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

    [SuppressMessage("", "S2325",
        Justification = "Placeholder that will map instance state once merge options are supported")]
    internal MergeOptions ToMergeOptions()
    {
        return new MergeOptions();
    }
}
