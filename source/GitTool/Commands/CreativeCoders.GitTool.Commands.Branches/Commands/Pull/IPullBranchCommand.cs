using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Pull;

public interface IPullBranchCommand
{
    Task<int> ExecuteAsync();
}
