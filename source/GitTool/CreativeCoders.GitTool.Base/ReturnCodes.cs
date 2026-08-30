namespace CreativeCoders.GitTool.Base;

/// <summary>
/// Defines the exit codes returned by the git tool commands.
/// </summary>
public static class ReturnCodes
{
    /// <summary>
    /// The command completed successfully.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// The command failed for a reason which has no dedicated return code.
    /// </summary>
    public const int GeneralError = -1;

    /// <summary>
    /// A merge could not be completed because of conflicts.
    /// </summary>
    public const int MergeConflicts = -3;

    /// <summary>
    /// The command needs a clean working tree, but the branch has uncommitted changes.
    /// </summary>
    public const int BranchHasUncommittedChanges = -4;

    /// <summary>
    /// No git repository was found for the current directory.
    /// </summary>
    public const int NoGitRepositoryFound = -5;

    /// <summary>
    /// No feature branch was found for the requested operation.
    /// </summary>
    public const int NoFeatureBranchFound = -6;

    /// <summary>
    /// The feature branch to be created already exists locally.
    /// </summary>
    public const int FeatureBranchAlreadyExistsLocal = -7;

    /// <summary>
    /// The feature branch to be created already exists on the remote.
    /// </summary>
    public const int FeatureBranchAlreadyExistsRemote = -8;

    /// <summary>
    /// The requested tag does not exist in the local repository.
    /// </summary>
    public const int LocalTagNotFound = -9;

    /// <summary>
    /// The creation of the release was aborted by the user.
    /// </summary>
    public const int ReleaseCreationAborted = -10;

    /// <summary>
    /// Deleting at least one local branch failed.
    /// </summary>
    public const int DeleteLocalBranchFailed = -11;

    /// <summary>
    /// The command needs an interactive terminal, but none is available.
    /// </summary>
    public const int NoInteractiveTerminal = -12;
}
