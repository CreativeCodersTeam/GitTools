using CreativeCoders.Git.Abstractions.Common;

namespace CreativeCoders.Git.Common;

/// <summary>
/// Represents a Git signature containing name, email, and timestamp information.
/// </summary>
public class GitSignature : IGitSignature
{
    private readonly Signature _signature;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitSignature"/> class.
    /// </summary>
    /// <param name="signature">The underlying LibGit2Sharp signature.</param>
    public GitSignature(Signature signature)
    {
        _signature = Ensure.NotNull(signature);
    }

    /// <inheritdoc />
    public DateTimeOffset When => _signature.When;

    /// <inheritdoc />
    public string Name => _signature.Name;

    /// <inheritdoc />
    public string Email => _signature.Email;
}
