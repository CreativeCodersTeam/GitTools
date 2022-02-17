using System.Threading.Tasks;
using CreativeCoders.GitTool.Commands.Branches.Commands.List;
using CreativeCoders.GitTool.Commands.Branches.Commands.Update;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Commands.Branches
{
    [CliController("branch")]
    public class BranchesController
    {
        private readonly IListBranchesCommand _listBranchesCommand;

        private readonly IUpdateBranchesCommand _updateBranchesCommand;

        public BranchesController(IListBranchesCommand listBranchesCommand,
            IUpdateBranchesCommand updateBranchesCommand)
        {
            _listBranchesCommand = listBranchesCommand;
            _updateBranchesCommand = updateBranchesCommand;
        }

        [UsedImplicitly]
        [CliAction]
        [CliAction("list")]
        public async Task<CliActionResult> ListAsync(ListBranchesOptions options)
        {
            var result = await _listBranchesCommand.ExecuteAsync(options);

            return new CliActionResult(result);
        }

        [UsedImplicitly]
        [CliAction("update")]
        public async Task<int> UpdateAsync(UpdateBranchesOptions options)
        {
            return await _updateBranchesCommand.ExecuteAsync(options);
        }
    }
}
