using System.Diagnostics.CodeAnalysis;

namespace CreativeCoders.Git.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public class GitRepositoryOptions
    {
        public string Path { get; set; } = string.Empty;
    }
}
