using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

[PublicAPI]
public class GitPushFailedException : GitException
{
    public GitPushFailedException()
    {
    }

    public GitPushFailedException(string? message) : base(message)
    {
    }

    public GitPushFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}