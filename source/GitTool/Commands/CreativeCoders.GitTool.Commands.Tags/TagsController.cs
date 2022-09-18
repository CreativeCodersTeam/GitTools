using CreativeCoders.Core;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;
using CreativeCoders.GitTool.Commands.Tags.Commands.ListTags;
using CreativeCoders.SysConsole.Cli.Actions;
using CreativeCoders.SysConsole.Cli.Actions.Definition;

namespace CreativeCoders.GitTool.Commands.Tags;

[CliController("tags")]
public class TagsController
{
    private readonly IGitToolCommandExecutor _commandExecutor;

    public TagsController(IGitToolCommandExecutor commandExecutor)
    {
        _commandExecutor = Ensure.NotNull(commandExecutor, nameof(commandExecutor));
    }

    [CliAction]
    [CliAction("list")]
    public async Task<CliActionResult> ListAsync(ListTagsOptions options)
        => new(await _commandExecutor.ExecuteAsync<ListTagsCommand, ListTagsOptions>(options));
}
