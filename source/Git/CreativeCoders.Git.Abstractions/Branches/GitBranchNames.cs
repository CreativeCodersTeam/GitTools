namespace CreativeCoders.Git.Abstractions.Branches;

/// <summary>   Static classes with constants an methods for default main branch names. </summary>
public static class GitBranchNames
{
    /// <summary>   Names for local main/master branch. </summary>
    public static class Local
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets friendly name for <paramref name="mainBranch"/>. </summary>
        ///
        /// <param name="mainBranch">   The main branch. </param>
        ///
        /// <returns>
        ///     The friendly name or string.Empty if <paramref name="mainBranch"/> is
        ///     <see cref="GitMainBranch.Custom"/>.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public static string GetFriendlyName(GitMainBranch mainBranch)
        {
            return mainBranch switch
            {
                GitMainBranch.Main => Main.FriendlyName,
                GitMainBranch.Master => Master.FriendlyName,
                _ => string.Empty
            };
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets canonical name name for <paramref name="mainBranch"/>. </summary>
        ///
        /// <param name="mainBranch">   The main branch. </param>
        ///
        /// <returns>
        ///     The canonical name or string.Empty if <paramref name="mainBranch"/> is
        ///     <see cref="GitMainBranch.Custom"/>.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public static string GetCanonicalName(GitMainBranch mainBranch)
        {
            return mainBranch switch
            {
                GitMainBranch.Main => Main.CanonicalName,
                GitMainBranch.Master => Master.CanonicalName,
                _ => string.Empty
            };
        }

        /// <summary>   Names for local master branch. </summary>
        public static class Master
        {
            /// <summary>   Friendly name for local master branch. </summary>
            public const string FriendlyName = "master";

            /// <summary>   Canonical name for local master branch. </summary>
            public const string CanonicalName = "refs/heads/master";
        }

        /// <summary>   Names for local main branch. </summary>
        public static class Main
        {
            /// <summary>   Friendly name for local main branch. </summary>
            public const string FriendlyName = "main";

            /// <summary>   Canonical name for local main branch. </summary>
            public const string CanonicalName = "refs/heads/main";
        }
    }

    /// <summary>   Names for remote main/master branch. </summary>
    public static class Remote
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets friendly name for <paramref name="mainBranch"/>. </summary>
        ///
        /// <param name="mainBranch">   The main branch. </param>
        ///
        /// <returns>
        ///     The friendly name or string.Empty if <paramref name="mainBranch"/> is
        ///     <see cref="GitMainBranch.Custom"/>.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public static string GetFriendlyName(GitMainBranch mainBranch)
        {
            return mainBranch switch
            {
                GitMainBranch.Main => Main.FriendlyName,
                GitMainBranch.Master => Master.FriendlyName,
                _ => string.Empty
            };
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets canonical name for <paramref name="mainBranch"/>. </summary>
        ///
        /// <param name="mainBranch">   The main branch. </param>
        ///
        /// <returns>
        ///     The canonical name or string.Empty if <paramref name="mainBranch"/> is
        ///     <see cref="GitMainBranch.Custom"/>.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public static string GetCanonicalName(GitMainBranch mainBranch)
        {
            return mainBranch switch
            {
                GitMainBranch.Main => Main.CanonicalName,
                GitMainBranch.Master => Master.CanonicalName,
                _ => string.Empty
            };
        }

        /// <summary>   Names for remote master branch. </summary>
        public static class Master
        {
            /// <summary>   Friendly name for remote master branch. </summary>
            public const string FriendlyName = "origin/master";

            /// <summary>   Canonical name for remote master branch. </summary>
            public const string CanonicalName = "refs/remotes/origin/master";
        }

        /// <summary>   Names for remote main branch. </summary>
        public static class Main
        {
            /// <summary>   Friendly name for remote main branch. </summary>
            public const string FriendlyName = "origin/main";

            /// <summary>   Canonical name for remote main branch. </summary>
            public const string CanonicalName = "refs/remotes/origin/main";
        }
    }
}