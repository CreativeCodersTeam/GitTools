using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions.Remotes;
using LibGit2Sharp;

namespace CreativeCoders.Git.Remotes;

public class GitRemoteCollection : IGitRemoteCollection
{
    private readonly RemoteCollection _remoteCollection;

    public GitRemoteCollection(RemoteCollection remoteCollection)
    {
        _remoteCollection = Ensure.NotNull(remoteCollection, nameof(remoteCollection));
    }

    public IEnumerator<IGitRemote> GetEnumerator()
        => _remoteCollection.Select(x => new GitRemote(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IGitRemote? this[string name] => GitRemote.From(_remoteCollection[name]);
}