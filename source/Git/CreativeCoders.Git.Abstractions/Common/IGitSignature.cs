using System;

namespace CreativeCoders.Git.Abstractions.Common;

public interface IGitSignature
{
    public DateTimeOffset When { get; }

    public string Name { get; }

    public string Email { get; }
}
