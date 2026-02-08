namespace CreativeCoders.Git.Abstractions.GitCommands;

public interface IFetchTagsCommand
{
    void Execute(string remoteName = "origin");
}
