namespace CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

public interface IGitToolCommandExecutor
{
    Task<int> ExecuteAsync<TCommand>()
        where TCommand : IGitToolCommand;

    Task<int> ExecuteAsync<TCommand, TCommandOptions>(TCommandOptions options)
        where TCommand : IGitToolCommandWithOptions<TCommandOptions>;
}
