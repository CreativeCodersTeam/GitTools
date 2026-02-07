namespace CreativeCoders.GitTool.Base;

public static class ReturnCodes
{
    public const int Success = 0;

    public const int GeneralError = -1;

    public const int MergeConflicts = -3;

    public const int BranchHasUncommittedChanges = -4;

    public const int NoGitRepositoryFound = -5;

    public const int NoFeatureBranchFound = -6;

    public const int FeatureBranchAlreadyExistsLocal = -7;

    public const int FeatureBranchAlreadyExistsRemote = -8;
}
