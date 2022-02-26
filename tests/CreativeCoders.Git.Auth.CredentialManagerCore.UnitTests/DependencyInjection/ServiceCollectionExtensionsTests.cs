using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Auth.CredentialManagerCore.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CreativeCoders.Git.Auth.CredentialManagerCore.UnitTests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddGcmCoreCredentialProvider_NoArgs_RegistersDefaultGcmCoreCredentialProvider()
    {
        var services = new ServiceCollection();

        // Act
        services.AddGcmCoreCredentialProvider();

        var sp = services.BuildServiceProvider();

        // Assert
        var provider = sp.GetRequiredService<IGitCredentialProvider>();

        provider
            .Should()
            .BeOfType<DefaultGcmCoreCredentialProvider>();
    }
}