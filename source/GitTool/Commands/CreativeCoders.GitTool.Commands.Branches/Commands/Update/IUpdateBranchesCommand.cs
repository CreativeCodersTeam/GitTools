using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Update;

public interface IUpdateBranchesCommand
{
    Task<int> ExecuteAsync(UpdateBranchesOptions options);
}