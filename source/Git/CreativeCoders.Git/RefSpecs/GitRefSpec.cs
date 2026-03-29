using CreativeCoders.Git.Abstractions.RefSpecs;
using CreativeCoders.Git.Common;

namespace CreativeCoders.Git.RefSpecs;

/// <summary>
/// Represents a Git reference specification that defines the mapping between remote and local references.
/// </summary>
public class GitRefSpec : ComparableObject<GitRefSpec, IGitRefSpec>, IGitRefSpec
{
    private readonly RefSpec _refSpec;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitRefSpec"/> class.
    /// </summary>
    /// <param name="refSpec">The underlying LibGit2Sharp ref spec.</param>
    internal GitRefSpec(RefSpec refSpec)
    {
        _refSpec = Ensure.NotNull(refSpec);
    }

    static GitRefSpec() => InitComparableObject(x => x.Specification);

    /// <inheritdoc />
    public string Specification => _refSpec.Specification;

    /// <inheritdoc />
    public GitRefSpecDirection Direction => _refSpec.Direction.ToGitRefSpecDirection();

    /// <inheritdoc />
    public string Source => _refSpec.Source;

    /// <inheritdoc />
    public string Destination => _refSpec.Destination;

    /// <inheritdoc />
    public override string ToString() => Specification;

    /// <summary>
    /// Converts a <see cref="GitRefSpec"/> to a LibGit2Sharp <see cref="RefSpec"/>.
    /// </summary>
    /// <param name="refSpec">The Git ref spec to convert.</param>
    public static implicit operator RefSpec(GitRefSpec refSpec) => refSpec._refSpec;
}
