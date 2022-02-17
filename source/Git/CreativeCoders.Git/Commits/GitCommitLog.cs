using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Commits;
using LibGit2Sharp;

namespace CreativeCoders.Git.Commits
{
    public class GitCommitLog : IGitCommitLog
    {
        private readonly ICommitLog _commitLog;

        internal GitCommitLog(ICommitLog commitLog)
        {
            _commitLog = Ensure.NotNull(commitLog, nameof(commitLog));
        }

        internal static GitCommitLog? From(ICommitLog? commitLog)
        {
            return commitLog == null
                ? null
                : new GitCommitLog(commitLog);
        }

        public IEnumerator<IGitCommit> GetEnumerator()
            => _commitLog.Select(x => new GitCommit(x)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IGitCommit> GetCommitsPriorTo(DateTimeOffset olderThan)
            => this.SkipWhile(c => c.When > olderThan);

        public IEnumerable<IGitCommit> QueryBy(GitCommitFilter commitFilter)
        {
            throw new NotImplementedException();
        }
    }
}
