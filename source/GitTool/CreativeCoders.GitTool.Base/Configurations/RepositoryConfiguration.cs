using CreativeCoders.Git.Abstractions.Branches;

namespace CreativeCoders.GitTool.Base.Configurations
{
    public class RepositoryConfiguration
    {
        public string GetDefaultBranchName(GitMainBranch gitMainBranch)
        {
            return HasDevelopBranch
                ? DevelopBranch
                : GitBranchNames.Local.GetFriendlyName(gitMainBranch);
        }

        public string GetFeatureBranchName(string feature)
        {
            return $"{FeatureBranchPrefix}{feature}";
        }

        public bool HasDevelopBranch { get; set; }

        public string DevelopBranch { get; set; } = string.Empty;

        public string FeatureBranchPrefix { get; set; } = "feature/";

        public string GitServiceProviderName { get; set; } = string.Empty;

        public static RepositoryConfiguration Default { get; } = new()
        {
            HasDevelopBranch = true,
            DevelopBranch = "develop"
        };
    }
}
