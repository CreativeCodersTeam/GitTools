using System;
using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.Common;
using CreativeCoders.Git.Abstractions.Objects;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Commits;

[PublicAPI]
public interface IGitCommit : IEquatable<IGitCommit>, IComparable<IGitCommit>, IGitObject
{
    IEnumerable<IGitCommit> Parents { get; }

    IGitSignature Author { get; }

    IGitSignature Committer { get; }

    string Message { get; }
}