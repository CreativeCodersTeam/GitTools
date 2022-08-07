using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared;

public interface IGitToolPullCommand
{
    Task<int> ExecuteAsync(IGitRepository gitRepository, bool verbose);

    Task<int> ExecuteAsync(IGitRepository gitRepository);
}
