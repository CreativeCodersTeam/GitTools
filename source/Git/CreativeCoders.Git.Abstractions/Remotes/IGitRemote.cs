using System.Collections.Generic;
using CreativeCoders.Git.Abstractions.RefSpecs;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Remotes;

[PublicAPI]
public interface IGitRemote
{
    string Name { get; }

    string Url { get; }

    string PushUrl { get; }

    IEnumerable<IGitRefSpec> RefSpecs { get; }

    IEnumerable<IGitRefSpec> FetchRefSpecs { get; }

    IEnumerable<IGitRefSpec> PushRefSpecs { get; }
}