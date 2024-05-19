using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using GitCredentialManager;
using GitCredentialManager.Interop.Linux;
using GitCredentialManager.Interop.Windows;

namespace CreativeCoders.Git.Auth.CredentialManagerCore;

[ExcludeFromCodeCoverage]
internal class DefaultGcmCoreCredentialStore : IGcmCoreCredentialStore
{
    public ICredentialStore Create(string? credentialsNameSpace = default)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsCredentialManager(credentialsNameSpace);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new SecretServiceCollection(credentialsNameSpace);
        }

        throw new PlatformNotSupportedException();
    }
}
