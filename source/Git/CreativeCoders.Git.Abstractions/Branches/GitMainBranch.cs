namespace CreativeCoders.Git.Abstractions.Branches;

/// <summary>   Values that represent the git main branch. </summary>
public enum GitMainBranch
{
    /// <summary>   The main branch must be a custom branch name. </summary>
    Custom,

    /// <summary>   The main branch is 'master'. </summary>
    Master,

    /// <summary>   The main branch is 'main'. </summary>
    Main
}