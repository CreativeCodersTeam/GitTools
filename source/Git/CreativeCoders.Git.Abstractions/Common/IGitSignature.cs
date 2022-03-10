using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Common;

[PublicAPI]
public interface IGitSignature
{
    public DateTimeOffset When { get; }

    public string Name { get; }

    public string Email { get; }
}
