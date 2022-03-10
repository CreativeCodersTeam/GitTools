using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

[PublicAPI]
public class GitCommitFilter
{
    public bool FirstParentOnly { get; set; }

    public object? IncludeReachableFrom { get; set; }

    public object? ExcludeReachableFrom { get; set; }

    public GitCommitSortStrategies SortBy { get; set; }
}