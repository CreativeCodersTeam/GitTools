namespace CreativeCoders.Git.Abstractions.Fetches;

public class GitTransferProgress
{
    public int IndexedObjects { get; init; }

    public int ReceivedObjects { get; init; }

    public int TotalObjects { get; init; }

    public long ReceivedBytes { get; init; }
}
