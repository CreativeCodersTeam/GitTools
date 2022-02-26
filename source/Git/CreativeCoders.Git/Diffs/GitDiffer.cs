using System.Collections.Generic;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Diffs;
using LibGit2Sharp;

namespace CreativeCoders.Git.Diffs;

public class GitDiffer : IGitDiffer
{
    private readonly Diff _diff;

    public GitDiffer(Diff diff)
    {
        _diff = Ensure.NotNull(diff, nameof(diff));
    }

    public IGitTreeChanges Compare()
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>());
    }

    public IGitTreeChanges Compare(bool includeUntracked)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(null, includeUntracked));
    }

    public IGitTreeChanges Compare(IEnumerable<string> paths)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(paths));
    }

    public IGitTreeChanges Compare(IEnumerable<string> paths, bool includeUntracked)
    {
        return new GitTreeChanges(_diff.Compare<TreeChanges>(paths, includeUntracked));
    }
}