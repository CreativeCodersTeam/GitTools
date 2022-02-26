using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

[PublicAPI]
public interface IGitCommitLog : IEnumerable<IGitCommit>
{
    IEnumerable<IGitCommit> GetCommitsPriorTo(DateTimeOffset olderThan);

    IEnumerable<IGitCommit> QueryBy(GitCommitFilter commitFilter);
}