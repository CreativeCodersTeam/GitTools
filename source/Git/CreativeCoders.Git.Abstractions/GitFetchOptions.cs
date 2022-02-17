namespace CreativeCoders.Git.Abstractions
{
    public class GitFetchOptions
    {
        public bool? Prune { get; set; }

        public GitTagFetchMode? TagFetchMode { get; set; }

        public string[]? CustomHeaders { get; set; }
    }
}
