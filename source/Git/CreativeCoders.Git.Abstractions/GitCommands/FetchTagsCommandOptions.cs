namespace CreativeCoders.Git.Abstractions.GitCommands;

public class FetchTagsCommandOptions
{
    public bool Prune { get; set; } = true;

    public string RemoteName { get; set; } = "origin";
}
