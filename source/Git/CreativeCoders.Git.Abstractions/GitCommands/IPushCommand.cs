using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.GitCommands;

[PublicAPI]
public interface IPushCommand
{
    IPushCommand CreateRemoteBranchIfNotExists();

    IPushCommand CreateRemoteBranchIfNotExists(bool createRemoteBranchIfNotExists);

    IPushCommand Branch(IGitBranch branch);

    void Run();
}
