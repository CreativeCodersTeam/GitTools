using CreativeCoders.Git.Abstractions.RefSpecs;
using CreativeCoders.Git.Abstractions.Remotes;
using CreativeCoders.Git.RefSpecs;

namespace CreativeCoders.Git.Remotes;

public class GitRemote : ComparableObject<GitRemote, IGitRemote>, IGitRemote
{
    private readonly Remote _remote;

    static GitRemote() => InitComparableObject(x => x.Name);

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

    public static implicit operator Remote(GitRemote remote) => remote._remote;

    public string Name => _remote.Name;

    public string Url => _remote.Url;

    public string PushUrl => _remote.PushUrl;

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

    public IEnumerable<IGitRefSpec> FetchRefSpecs => _remote.FetchRefSpecs.Select(x => new GitRefSpec(x));

    public IEnumerable<IGitRefSpec> PushRefSpecs => _remote.PushRefSpecs.Select(x => new GitRefSpec(x));
}
