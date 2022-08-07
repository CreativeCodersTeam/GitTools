using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

public interface IGitToolCommandWithOptions<TOptions>
{
    Task<int> ExecuteAsync(IGitRepository gitRepository, TOptions options);
}
