using System;
using CreativeCoders.Core;
using CreativeCoders.Core.Comparing;

namespace CreativeCoders.Git.Abstractions.Common;

/// <summary>
/// Represents a parsed Git reference name with its canonical, friendly, and remote-stripped forms.
/// </summary>
public class ReferenceName : ComparableObject<ReferenceName>
{
    static ReferenceName()
    {
        InitComparableObject(x => x.Canonical);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceName"/> class.
    /// </summary>
    /// <param name="canonical">The canonical reference name (e.g. <c>refs/heads/main</c>).</param>
    public ReferenceName(string canonical)
    {
        Canonical = Ensure.IsNotNullOrWhitespace(canonical);
        Friendly = ShortenName();
        WithoutRemote = RemoveRemotePrefix();

        IsLocalBranch = IsPrefixedBy(Canonical, ReferencePrefixes.LocalBranch);
        IsRemoteBranch = IsPrefixedBy(Canonical, ReferencePrefixes.RemoteTrackingBranch);
        IsTag = IsPrefixedBy(Canonical, ReferencePrefixes.Tag);
        IsPullRequest = IsPrefixedBy(Canonical, ReferencePrefixes.PullRequest1)
                        || IsPrefixedBy(Canonical, ReferencePrefixes.PullRequest2);
    }

    /// <summary>
    /// Attempts to parse a canonical reference name.
    /// </summary>
    /// <param name="canonicalName">The canonical name to parse.</param>
    /// <param name="referenceName">When this method returns, contains the parsed reference name, or <see langword="null"/> if parsing failed. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true"/> if <paramref name="canonicalName"/> is a valid canonical reference name; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string canonicalName, out ReferenceName? referenceName)
    {
        if (IsPrefixedBy(canonicalName, ReferencePrefixes.LocalBranch)
            || IsPrefixedBy(canonicalName, ReferencePrefixes.RemoteTrackingBranch)
            || IsPrefixedBy(canonicalName, ReferencePrefixes.Tag)
            || IsPrefixedBy(canonicalName, ReferencePrefixes.PullRequest1)
            || IsPrefixedBy(canonicalName, ReferencePrefixes.PullRequest2))
        {
            referenceName = new ReferenceName(canonicalName);
            return true;
        }

        referenceName = null;
        return false;
    }

    /// <summary>
    /// Parses a canonical reference name.
    /// </summary>
    /// <param name="canonicalName">The canonical name to parse.</param>
    /// <returns>The parsed reference name.</returns>
    /// <exception cref="ArgumentException"><paramref name="canonicalName"/> is not a valid canonical reference name.</exception>
    public static ReferenceName Parse(string canonicalName)
    {
        return TryParse(canonicalName, out var referenceName)
            ? referenceName!
            : throw new ArgumentException($"'{nameof(canonicalName)}' is not a canonical name");
    }

    private string ShortenName()
    {
        if (IsPrefixedBy(Canonical, ReferencePrefixes.LocalBranch))
            return Canonical[ReferencePrefixes.LocalBranch.Length..];

        if (IsPrefixedBy(Canonical, ReferencePrefixes.RemoteTrackingBranch))
            return Canonical[ReferencePrefixes.RemoteTrackingBranch.Length..];

        return IsPrefixedBy(Canonical, ReferencePrefixes.Tag)
            ? Canonical[ReferencePrefixes.Tag.Length..]
            : Canonical;
    }

    private string RemoveRemotePrefix()
    {
        var isRemote = IsPrefixedBy(Canonical, ReferencePrefixes.RemoteTrackingBranch);

        return isRemote
            ? Friendly[(Friendly.IndexOf('/') + 1)..]
            : Friendly;
    }

    private static bool IsPrefixedBy(string input, string prefix)
        => input.StartsWith(prefix, StringComparison.Ordinal);

    /// <summary>
    /// Gets the full canonical name of the reference (e.g. <c>refs/heads/main</c>).
    /// </summary>
    public string Canonical { get; }

    /// <summary>
    /// Gets the friendly (shortened) name of the reference (e.g. <c>main</c>).
    /// </summary>
    public string Friendly { get; }

    /// <summary>
    /// Gets the friendly name without the remote prefix (e.g. <c>main</c> instead of <c>origin/main</c>).
    /// </summary>
    public string WithoutRemote { get; }

    /// <summary>
    /// Gets a value that indicates whether this reference is a local branch.
    /// </summary>
    /// <value><see langword="true"/> if this is a local branch; otherwise, <see langword="false"/>.</value>
    public bool IsLocalBranch { get; }

    /// <summary>
    /// Gets a value that indicates whether this reference is a remote-tracking branch.
    /// </summary>
    /// <value><see langword="true"/> if this is a remote-tracking branch; otherwise, <see langword="false"/>.</value>
    public bool IsRemoteBranch { get; }

    /// <summary>
    /// Gets a value that indicates whether this reference is a tag.
    /// </summary>
    /// <value><see langword="true"/> if this is a tag; otherwise, <see langword="false"/>.</value>
    public bool IsTag { get; }

    /// <summary>
    /// Gets a value that indicates whether this reference is a pull request.
    /// </summary>
    /// <value><see langword="true"/> if this is a pull request; otherwise, <see langword="false"/>.</value>
    public bool IsPullRequest { get; }
}
