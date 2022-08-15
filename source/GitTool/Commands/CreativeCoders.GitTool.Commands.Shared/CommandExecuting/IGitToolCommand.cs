using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

public interface IGitToolCommand
{
    Task<int> ExecuteAsync(IGitRepository gitRepository);
}
