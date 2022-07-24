namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IGitCommands
{
    IPullCommand CreatePullCommand();
}
