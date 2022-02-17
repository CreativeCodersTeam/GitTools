using System.Diagnostics.CodeAnalysis;

namespace CreativeCoders.Git.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class GitPushOptions
    {
        public bool CreateRemoteBranchIfNotExists { get; set; } = true;
    }
}
