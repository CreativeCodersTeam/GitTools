using System;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Commits;
using CreativeCoders.Git.Abstractions.Exceptions;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions;

[PublicAPI]
public static class GitRepositoryBranchExtensions
{
    public static IGitBranch? CreateBranch(this IGitRepository gitRepository, string sourceBranchName,
        string newBranchName, bool updateSourceBranchBefore)
    {
        var sourceBranch = gitRepository.CheckOut(sourceBranchName);

        if (sourceBranch == null)
        {
            throw new GitBranchNotExistsException(sourceBranchName);
        }

        if (updateSourceBranchBefore)
        {
            gitRepository.FetchAllTagsFromOrigin();

            gitRepository.Pull();
        }

        return gitRepository.CreateBranch(newBranchName);
    }

    public static bool BranchIsPushedToRemote(this IGitBranch branch)
    {
        return branch.IsRemote || branch.Tip?.Sha == branch.TrackedBranch?.Tip?.Sha;
    }

    public static IEnumerable<IGitCommit> UnPushedCommits(this IGitBranch branch)
    {
        if (branch.TrackedBranch == null || branch.Commits == null)
        {
            yield break;
        }

        foreach (var gitCommit in branch.Commits)
        {
            if (gitCommit.Id.Equals(branch.TrackedBranch.Tip?.Id))
            {
                yield break;
            }
            else
            {
                yield return gitCommit;
            }
        }
    }
}