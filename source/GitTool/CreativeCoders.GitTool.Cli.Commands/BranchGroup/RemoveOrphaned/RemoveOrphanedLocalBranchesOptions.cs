using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.RemoveOrphaned;

/// <summary>
/// Represents the options of the command which removes orphaned local branches.
/// </summary>
[PublicAPI]
public class RemoveOrphanedLocalBranchesOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether all orphaned local branches are deleted without selection.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to skip the interactive selection and delete all orphaned local branches;
    /// otherwise, <see langword="false"/>.
    /// </value>
    [OptionParameter('a', "all", HelpText = "Deletes all orphaned local branches without selection")]
    public bool All { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the fetch prune before determining the orphaned local
    /// branches is skipped.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to skip the fetch prune; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// Without the fetch prune, branches whose remote counterpart was deleted are not detected, because their
    /// remote refs still exist locally.
    /// </remarks>
    [OptionParameter("sp", "skip-fetch-prune",
        HelpText = "Skips the fetch prune before determining the orphaned local branches")]
    public bool SkipFetchPrune { get; set; }
}
