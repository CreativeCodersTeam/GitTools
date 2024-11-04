using CreativeCoders.Git.Abstractions.Branches;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Base.Configurations;

[PublicAPI]
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

    public bool DisableCertificateValidation { get; set; }

    public static RepositoryConfiguration Default { get; } = new RepositoryConfiguration
    {
        HasDevelopBranch = true,
        DevelopBranch = "develop"
    };
}
