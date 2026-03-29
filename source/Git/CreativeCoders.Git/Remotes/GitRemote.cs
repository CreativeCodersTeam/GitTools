using CreativeCoders.Git.Abstractions.RefSpecs;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.RefSpecs;

namespace CreativeCoders.Git.Remotes;

/// <summary>
/// Represents a Git remote repository reference.
/// </summary>
public class GitRemote : ComparableObject<GitRemote, IGitRemote>, IGitRemote
{
    private readonly Remote _remote;

    static GitRemote() => InitComparableObject(x => x.Name);

    /// <summary>
    /// Initializes a new instance of the <see cref="GitRemote"/> class.
    /// </summary>
    /// <param name="remote">The underlying LibGit2Sharp remote.</param>
    internal GitRemote(Remote remote)
    {
        _remote = Ensure.NotNull(remote);
    }

    internal static GitRemote? From(Remote? remote)
    {
        return remote == null
            ? null
            : new GitRemote(remote);
    }

    /// <summary>
    /// Converts a <see cref="GitRemote"/> to a LibGit2Sharp <see cref="Remote"/>.
    /// </summary>
    /// <param name="remote">The Git remote to convert.</param>
    public static implicit operator Remote(GitRemote remote) => remote._remote;

    /// <inheritdoc />
    public string Name => _remote.Name;

    /// <inheritdoc />
    public string Url => _remote.Url;

    /// <inheritdoc />
    public string PushUrl => _remote.PushUrl;

    /// <inheritdoc />
    public IEnumerable<IGitRefSpec> RefSpecs
    {
        get
        {
            var refSpecs = _remote.RefSpecs;

            return refSpecs is null
                ? []
                : new GitRefSpecCollection((RefSpecCollection)refSpecs);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IGitRefSpec> FetchRefSpecs => _remote.FetchRefSpecs.Select(x => new GitRefSpec(x));

    /// <inheritdoc />
    public IEnumerable<IGitRefSpec> PushRefSpecs => _remote.PushRefSpecs.Select(x => new GitRefSpec(x));
}
