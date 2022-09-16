using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Tags;

namespace CreativeCoders.Git.Tags;

public class GitTagCollection : IGitTagCollection
{
    private readonly Repository _repository;

    private readonly ILibGitCaller _libGitCaller;

    private readonly RepositoryContext _context;

    internal GitTagCollection(RepositoryContext context)
    {
        _context = Ensure.NotNull(context, nameof(context));
        _repository = _context.Repository;
        _libGitCaller = _context.LibGitCaller;
    }

    public IGitTag CreateTag(string tagName)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName)));
    }

    public IGitTag CreateTag(string tagName, string objectish)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName, objectish)));
    }

    public IGitTag CreateTagWithMessage(string tagName, string message)
    {
        return new GitTag(_libGitCaller.Invoke(() => _repository.ApplyTag(tagName, _context.GetSignature(), message)));
    }

    public IGitTag CreateTagWithMessage(string tagName, string objectish, string message)
    {
        return new GitTag(
            _libGitCaller
                .Invoke(() => _repository.ApplyTag(tagName, objectish, _context.GetSignature(), message)));
    }

    public void PushTag(IGitTag tag)
    {
        var pushOptions = new PushOptions
        {
            CredentialsProvider = _context.GetCredentialsHandler()
        };

        _libGitCaller.Invoke(() =>
            _repository.Network.Push(_repository.Network.Remotes[GitRemotes.Origin], tag.Name.Canonical, pushOptions));
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