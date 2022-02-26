using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Remotes;

[PublicAPI]
public interface IGitRemoteCollection : IEnumerable<IGitRemote>
{
    IGitRemote? this[string name] { get; }

    //void Remove(string remoteName);

    //void Update(string remoteName, string refSpec);
}