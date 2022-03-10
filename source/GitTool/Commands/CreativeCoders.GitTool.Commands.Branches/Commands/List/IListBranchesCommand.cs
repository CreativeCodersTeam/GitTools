using System.Threading.Tasks;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.List;

public interface IListBranchesCommand
{
    Task<int> ExecuteAsync(ListBranchesOptions options);
}