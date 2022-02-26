using CreativeCoders.Core;
using CreativeCoders.Core.Comparing;
using CreativeCoders.Git.Abstractions.RefSpecs;
using CreativeCoders.Git.Common;
using LibGit2Sharp;

namespace CreativeCoders.Git.RefSpecs;

public class GitRefSpec : ComparableObject<GitRefSpec, IGitRefSpec>, IGitRefSpec
{
    private readonly RefSpec _refSpec;

    internal GitRefSpec(RefSpec refSpec)
    {
        _refSpec = Ensure.NotNull(refSpec, nameof(refSpec));
    }

    static GitRefSpec() => InitComparableObject(x => x.Specification);

    public string Specification => _refSpec.Specification;

    public GitRefSpecDirection Direction => _refSpec.Direction.ToGitRefSpecDirection();

    public string Source => _refSpec.Source;

    public string Destination => _refSpec.Destination;

    public override string ToString() => Specification;

    public static implicit operator RefSpec(GitRefSpec refSpec) => refSpec._refSpec;
}