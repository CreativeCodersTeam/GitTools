using System;
using CreativeCoders.Core;
using CreativeCoders.Core.Comparing;

namespace CreativeCoders.Git.Abstractions.Common
{
    public class ReferenceName : ComparableObject<ReferenceName>
    {
        public ReferenceName(string canonical)
        {
            Canonical = Ensure.IsNotNullOrWhitespace(canonical, nameof(canonical));
            Friendly = ShortenName();
            WithoutRemote = RemoveRemotePrefix();

            IsLocalBranch = IsPrefixedBy(Canonical, ReferencePrefixes.LocalBranch);
            IsRemoteBranch = IsPrefixedBy(Canonical, ReferencePrefixes.RemoteTrackingBranch);
            IsTag = IsPrefixedBy(Canonical, ReferencePrefixes.Tag);
            IsPullRequest = IsPrefixedBy(Canonical, ReferencePrefixes.PullRequest1)
                            || IsPrefixedBy(Canonical, ReferencePrefixes.PullRequest2);
        }

        static ReferenceName()
        {
            InitComparableObject(x => x.Canonical);
        }

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

        public static ReferenceName Parse(string canonicalName)
        {
            if (TryParse(canonicalName, out var referenceName))
            {
                return referenceName!;
            }

            throw new ArgumentException($"'{nameof(canonicalName)}' is not a canonical name");
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
                ? Friendly[(Friendly.IndexOf("/", StringComparison.Ordinal) + 1)..]
                : Friendly;
        }

        private static bool IsPrefixedBy(string input, string prefix)
            => input.StartsWith(prefix, StringComparison.Ordinal);

        public string Canonical { get; }

        public string Friendly { get; }

        public string WithoutRemote { get; }

        public bool IsLocalBranch { get; }

        public bool IsRemoteBranch { get; }

        public bool IsTag { get; }

        public bool IsPullRequest { get; }
    }
}
