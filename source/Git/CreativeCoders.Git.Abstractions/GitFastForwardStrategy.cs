namespace CreativeCoders.Git.Abstractions;

public enum GitFastForwardStrategy
{
    /// <summary>
    ///     Default fast-forward strategy.  If the merge.ff configuration option is set,
    ///     it will be used.  If it is not set, this will perform a fast-forward merge if
    ///     possible, otherwise a non-fast-forward merge that results in a merge commit.
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Do not fast-forward. Always creates a merge commit.
    /// </summary>
    NoFastForward = 1,

    /// <summary>
    ///     Only perform fast-forward merges.
    /// </summary>
    FastForwardOnly = 2
}
