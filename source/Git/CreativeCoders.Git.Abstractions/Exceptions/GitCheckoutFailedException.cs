﻿using System;
using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Exceptions;

[PublicAPI]
public class GitCheckoutFailedException : GitException
{
    public GitCheckoutFailedException()
    {
    }

    public GitCheckoutFailedException(string? branchName) : base($"Checkout branch '{branchName}' failed")
    {
    }

    public GitCheckoutFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}