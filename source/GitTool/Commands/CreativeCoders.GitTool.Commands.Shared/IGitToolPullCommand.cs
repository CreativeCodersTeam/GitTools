using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared;

public interface IGitToolPullCommand
{
    void Run(IGitRepository gitRepository);
}
