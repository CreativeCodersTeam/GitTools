using CreativeCoders.SysConsole.Cli.Parsing;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Cli.Commands.BranchGroup.Update;

/// <summary>
/// Represents the options of the command which updates the permanent local branches.
/// </summary>
[PublicAPI]
public class UpdateBranchesOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether the fetch prune before the update is skipped.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to skip the fetch prune; otherwise, <see langword="false"/>.
    /// </value>
    [OptionParameter("skip-fetch-prune", HelpText = "Skips the fetch prune before updating the branches")]
    public bool SkipFetchPrune { get; set; }
}
