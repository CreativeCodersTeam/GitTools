using CreativeCoders.Git.Abstractions.Remotes;

namespace CreativeCoders.Git.Remotes;

/// <summary>
/// Represents a collection of Git remotes wrapping a LibGit2Sharp <see cref="RemoteCollection"/>.
/// </summary>
public class GitRemoteCollection : IGitRemoteCollection
{
    private readonly RemoteCollection _remoteCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitRemoteCollection"/> class.
    /// </summary>
    /// <param name="remoteCollection">The underlying LibGit2Sharp remote collection.</param>
    public GitRemoteCollection(RemoteCollection remoteCollection)
    {
        _remoteCollection = Ensure.NotNull(remoteCollection);
    }

    public IEnumerator<IGitRemote> GetEnumerator()
        => _remoteCollection.Select(x => new GitRemote(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IGitRemote? this[string name] => GitRemote.From(_remoteCollection[name]);
}
