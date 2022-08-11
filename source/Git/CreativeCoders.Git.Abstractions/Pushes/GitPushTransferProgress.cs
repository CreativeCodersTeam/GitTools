namespace CreativeCoders.Git.Abstractions.Pushes;

public class GitPushTransferProgress
{
    public GitPushTransferProgress(int current, int total, long bytes)
    {
        Current = current;
        Total = total;
        Bytes = bytes;
    }

    public int Current { get; }

    public int Total { get; }

    public long Bytes { get; }
}
