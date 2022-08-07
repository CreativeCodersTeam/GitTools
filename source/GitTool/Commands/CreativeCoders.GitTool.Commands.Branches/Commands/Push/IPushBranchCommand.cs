using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

public interface IPushBranchCommand
{
    Task<int> ExecuteAsync();
}