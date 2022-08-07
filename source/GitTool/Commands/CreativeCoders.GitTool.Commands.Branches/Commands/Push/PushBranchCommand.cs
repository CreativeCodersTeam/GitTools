using System.Threading.Tasks;
using CreativeCoders.Git.Abstractions;
using CreativeCoders.GitTool.Commands.Shared.CommandExecuting;

namespace CreativeCoders.GitTool.Commands.Branches.Commands.Push;

public class PushBranchCommand : IGitToolCommandWithOptions<PushBranchOptions>
{
    public Task<int> ExecuteAsync(IGitRepository gitRepository, PushBranchOptions options)
    {
        //gitRepository.Push(new GitPushOptions(){CreateRemoteBranchIfNotExists = });

        return Task.FromResult(0);
    }
}
