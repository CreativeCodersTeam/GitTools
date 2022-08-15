using CreativeCoders.Git.Abstractions;

namespace CreativeCoders.GitTool.Commands.Shared;

public interface IGitToolPushCommand
{
    Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists, bool verbose);

    Task<int> ExecuteAsync(IGitRepository gitRepository, bool createRemoteIsNotExists);
}
