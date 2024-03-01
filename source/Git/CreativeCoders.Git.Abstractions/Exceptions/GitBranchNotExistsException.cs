using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

[PublicAPI]
[ExcludeFromCodeCoverage]
public class GitBranchNotExistsException : GitException
{
    public GitBranchNotExistsException()
    {
    }

    public GitBranchNotExistsException(string? branchName) : base($"Branch '{branchName}' does not exist")
    {
    }

    public GitBranchNotExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}