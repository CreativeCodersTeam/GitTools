using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Tags;

namespace CreativeCoders.Git.Tags;

public class GitTagCollection : IGitTagCollection
{
    private readonly RepositoryContext _context;

    private readonly ILibGitCaller _libGitCaller;

    private readonly Repository _repository;

    internal GitTagCollection(RepositoryContext context)
    {
        _context = Ensure.NotNull(context);
        _repository = _context.LibGitRepository;
        _libGitCaller = _context.LibGitCaller;
    }

    public IGitTag CreateTag(string tagName, string? objectish = null)
    {
        return string.IsNullOrWhiteSpace(objectish)
            ? new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName)))
            : new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName, objectish)));
    }

    public IGitTag CreateTagWithMessage(string tagName, string message, string? objectish = null)
    {
        return string.IsNullOrWhiteSpace(objectish)
            ? new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName, _context.GetSignature(), message)))
            : new GitTag(_libGitCaller
                .Invoke(() => _repository.ApplyTag(tagName, objectish, _context.GetSignature(), message)));
    }

    public void DeleteTag(string tagName, bool deleteOnRemote = false)
    {
        _libGitCaller.Invoke(() => _repository.Tags.Remove(tagName));

        if (deleteOnRemote)
        {
            DeleteRemoteTag(tagName);
        }
    }

    public void DeleteTag(IGitTag tag, bool deleteOnRemote = false)
    {
        DeleteTag(tag.Name.Canonical, deleteOnRemote);
    }

    public void DeleteRemoteTag(string tagName)
    {
        var pushOptions = new PushOptions
        {
            CredentialsProvider = _context.GetCredentialsHandler()
        };

        _libGitCaller.Invoke(() =>
            _repository.Network.Push(_repository.Network.Remotes[GitRemotes.Origin], $":refs/tags/{tagName}",
                pushOptions));
    }

    public void PushTag(string tagName)
    {
        var pushOptions = new PushOptions
        {
            CredentialsProvider = _context.GetCredentialsHandler()
        };

        _libGitCaller.Invoke(() =>
            _repository.Network.Push(_repository.Network.Remotes[GitRemotes.Origin], tagName, pushOptions));
    }

    public void PushTag(IGitTag tag)
    {
        PushTag(tag.Name.Canonical);
    }

    public void PushAllTags()
    {
        this.ForEach(PushTag);
    }

    public IEnumerator<IGitTag> GetEnumerator()
        => _repository.Tags.Select(x => new GitTag(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
