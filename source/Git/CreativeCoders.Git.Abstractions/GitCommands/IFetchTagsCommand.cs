namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IFetchTagsCommand
{
    void Execute(FetchTagsCommandOptions commandOptions);
}
