using System.Linq;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using Spectre.Console;

namespace CreativeCoders.GitTool.Commands.Releases.Commands.ChangeLog;

public class ChangeLogCommand : IGitToolCommandWithOptions<ChangeLogOptions>
{
    private readonly IAnsiConsole _console;

    public ChangeLogCommand(IAnsiConsole console)
    {
        _console = Ensure.NotNull(console, nameof(console));
    }

    public Task<int> ExecuteAsync(IGitRepository gitRepository, ChangeLogOptions options)
    {
        var tag = gitRepository.Tags.Reverse().FirstOrDefault();

        if (tag != null)
        {
            _console.WriteMarkupLine(tag.Name.Friendly);

            if (tag.TargetSha != gitRepository.Head.Tip.Sha)
            {
                _console.WriteMarkupLine("New commits");
            }
        }

        return Task.FromResult(0);
    }
}
