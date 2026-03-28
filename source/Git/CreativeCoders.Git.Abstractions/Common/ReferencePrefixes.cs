namespace CreativeCoders.Git.Abstractions.Common;

/// <summary>
/// Provides well-known prefix strings for Git reference names.
/// </summary>
public static class ReferencePrefixes
{
    /// <summary>
    /// The prefix for local branch references (<c>refs/heads/</c>).
    /// </summary>
    public const string LocalBranch = "refs/heads/";

    /// <summary>
    /// The prefix for remote-tracking branch references (<c>refs/remotes/</c>).
    /// </summary>
    public const string RemoteTrackingBranch = "refs/remotes/";

    /// <summary>
    /// The prefix for tag references (<c>refs/tags/</c>).
    /// </summary>
    public const string Tag = "refs/tags/";

    /// <summary>
    /// The prefix for pull request references using the GitHub convention (<c>refs/pull/</c>).
    /// </summary>
    public const string PullRequest1 = "refs/pull/";

    /// <summary>
    /// The prefix for pull request references using the Bitbucket convention (<c>refs/pull-requests/</c>).
    /// </summary>
    public const string PullRequest2 = "refs/pull-requests/";
}