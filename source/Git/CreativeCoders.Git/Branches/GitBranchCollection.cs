using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;

namespace CreativeCoders.Git.Branches;

public class GitBranchCollection : IGitBranchCollection
{
    private readonly BranchCollection _branchCollection;
    private readonly RepositoryContext _context;

    private readonly ILibGitCaller _libGitCaller;

    internal GitBranchCollection(RepositoryContext context)
    {
        _context = Ensure.NotNull(context);
        _branchCollection = _context.LibGitRepository.Branches;
        _libGitCaller = context.LibGitCaller;
    }

    public IGitBranch? CheckOut(string branchName)
    {
        Ensure.Argument(branchName).NotNullOrEmpty();

        var branch = this[branchName];

        if (branch == null)
        {
            throw new GitBranchNotExistsException(branchName);
        }

        var checkedOutBranch = _libGitCaller.Invoke(
            () => Commands.Checkout(_context.LibGitRepository, _context.LibGitRepository.Branches[branchName]));

        return GitBranch.From(checkedOutBranch);
    }

    public IGitBranch? CreateBranch(string branchName)
    {
        return GitBranch.From(_libGitCaller.Invoke(() => _context.LibGitRepository.CreateBranch(branchName)));
    }

    public void DeleteLocalBranch(string branchName)
    {
        var branch = _libGitCaller.Invoke(() => _context.LibGitRepository.Branches[branchName]);

        if (branch.IsRemote)
        {
            return;
        }

        _libGitCaller.Invoke(() => _context.LibGitRepository.Branches.Remove(branch));
    }

    public IEnumerator<IGitBranch> GetEnumerator()
        => _branchCollection.Select(x => new GitBranch(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IGitBranch? this[string name] => GitBranch.From(_branchCollection[name]);
}
