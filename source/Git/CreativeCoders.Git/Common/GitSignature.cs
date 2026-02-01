using CreativeCoders.Git.Abstractions.Common;

namespace CreativeCoders.Git.Common;

public class GitSignature : IGitSignature
{
    private readonly Signature _signature;

    public GitSignature(Signature signature)
    {
        _signature = Ensure.NotNull(signature);
    }

    public DateTimeOffset When => _signature.When;

    public string Name => _signature.Name;

    public string Email => _signature.Email;
}
