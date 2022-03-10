namespace CreativeCoders.GitTool.Base;

public static class ReturnCodes
{
    public const int Success = 0;

    public const int GeneralError = -1;

    public const int MergeConflicts = -3;

    public const int BranchHasUncommittedChanges = -4;
}