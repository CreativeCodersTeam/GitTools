using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using JetBrains.Annotations;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.CheckOut;

[UsedImplicitly]
public class CheckOutBranchesCommand : IGitToolCommandWithOptions<CheckOutBranchesOptions>
{
    private readonly IAnsiConsole _ansiConsole;
    
    private readonly ICml _cml;

    public CheckOutBranchesCommand(IAnsiConsole ansiConsole, ICml cml)
    {
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
        _cml = Ensure.NotNull(cml, nameof(cml));
    }
    
    public Task<int> ExecuteAsync(IGitRepository gitRepository, CheckOutBranchesOptions options)
    {
        _ansiConsole
            .WriteMarkupLine($"Check out branch '{_cml.HighLight(options.BranchName)}'")
            .EmptyLine();
        
        var branch = gitRepository.Branches.CheckOut(options.BranchName);

        if (branch != null)
        {
            _ansiConsole
                .WriteMarkupLine($"Branch '{_cml.HighLight(branch.Name.Friendly)}' checked out");
        }

        return Task.FromResult(0);
    }
}
