using CreativeCoders.Git.Abstractions.Branches;
using CreativeCoders.Git.Abstractions.Exceptions;

namespace CreativeCoders.Git.Branches;

public class GitBranchCollection : IGitBranchCollection
{
    private readonly RepositoryContext _context;

    private readonly BranchCollection _branchCollection;

    private readonly ILibGitCaller _libGitCaller;

    internal GitBranchCollection(RepositoryContext context)
    {
        _context = Ensure.NotNull(context, nameof(context));
        _branchCollection = _context.Repository.Branches;
        _libGitCaller = context.LibGitCaller;
    }

    public IGitBranch? CheckOut(string branchName)
    {
        Ensure.Argument(branchName, nameof(branchName)).NotNullOrEmpty();

        var branch = this[branchName];

        if (branch == null)
        {
            throw new GitBranchNotExistsException(branchName);
        }

        var checkedOutBranch = _libGitCaller.Invoke(
            () => Commands.Checkout(_context.Repository, _context.Repository.Branches[branchName]));

        return GitBranch.From(checkedOutBranch);
    }

    public IGitBranch? CreateBranch(string branchName)
    {
        return GitBranch.From(_libGitCaller.Invoke(() => _context.Repository.CreateBranch(branchName)));
    }

    public void DeleteLocalBranch(string branchName)
    {
        var branch = _libGitCaller.Invoke(() => _context.Repository.Branches[branchName]);

        if (branch.IsRemote)
        {
            return;
        }

        _libGitCaller.Invoke(() => _context.Repository.Branches.Remove(branch));
    }

    public IEnumerator<IGitBranch> GetEnumerator()
        => _branchCollection.Select(x => new GitBranch(x)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IGitBranch? this[string name] => GitBranch.From(_branchCollection[name]);
}