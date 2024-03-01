using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

[ExcludeFromCodeCoverage]
[PublicAPI]
public class GitException : Exception
{
    public GitException()
    {
    }

    public GitException(string? message) : base(message)
    {
    }

    public GitException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}