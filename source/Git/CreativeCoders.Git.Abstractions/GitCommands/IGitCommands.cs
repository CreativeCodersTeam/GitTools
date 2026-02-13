namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IGitCommands
{
    IPullCommand CreatePullCommand();

    IPushCommand CreatePushCommand();

    IFetchTagsCommand CreateFetchTagsCommand();
}
