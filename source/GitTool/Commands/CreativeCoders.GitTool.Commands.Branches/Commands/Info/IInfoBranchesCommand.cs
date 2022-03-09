using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Info;

public interface IInfoBranchesCommand
{
    Task<int> ExecuteAsync();
}