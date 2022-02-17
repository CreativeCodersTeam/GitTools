using System;
using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions
{
    [PublicAPI]
    public interface IGitRepositoryInfo
    {
        string? Path { get; }

        GitMainBranch MainBranch { get; }

        Uri RemoteUri { get; }
    }
}
