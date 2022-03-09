using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

public class InfoBranchesCommand : IInfoBranchesCommand
{
    private readonly IGitRepositoryFactory _gitRepositoryFactory;

    private readonly ISysConsole _sysConsole;

    public InfoBranchesCommand(IGitRepositoryFactory gitRepositoryFactory, ISysConsole sysConsole)
    {
        _gitRepositoryFactory = Ensure.NotNull(gitRepositoryFactory, nameof(gitRepositoryFactory));
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
    }

    public Task<int> ExecuteAsync()
    {
        using var gitRepository = _gitRepositoryFactory.OpenRepositoryFromCurrentDir();

        _sysConsole
            .WriteLine("Branch info")
            .WriteLine()
            .WriteLine($"Current branch: {gitRepository.Head.Name.Canonical}")
            .WriteLine($"Last Commit: {gitRepository.Head.Tip?.Sha}");

        if (gitRepository.Head.TrackedBranch != null)
        {
            _sysConsole.WriteLine($"Last tracked commit: {gitRepository.Head.TrackedBranch?.Tip?.Sha}");
        }

        _sysConsole.WriteLine();

        return Task.FromResult(0);
    }
}
