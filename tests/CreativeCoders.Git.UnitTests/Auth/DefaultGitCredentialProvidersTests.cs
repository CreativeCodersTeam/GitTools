using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Auth;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace CreativeCoders.Git.UnitTests.Auth;

public class DefaultGitCredentialProvidersTests
{
    [Fact]
    public void GetProvider_ForExistingName_ReturnsMatchingProvider()
    {
        const string providerName0 = "provider0";
        const string providerName1 = "provider1";

        var provider0 = A.Fake<IGitCredentialProvider>();
        var provider1 = A.Fake<IGitCredentialProvider>();

        A.CallTo(() => provider0.Name).Returns(providerName0);
        A.CallTo(() => provider1.Name).Returns(providerName1);

        var providers = new DefaultGitCredentialProviders(new[] { provider0, provider1 });

        // Act
        var provider = providers.GetProvider(providerName0);

        // Assert
        provider
            .Should()
            .BeSameAs(provider0);
    }

    [Fact]
    public void GetProvider_ForNotExistingName_ReturnsNull()
    {
        const string providerName0 = "provider0";
        const string providerName1 = "provider1";
        const string providerName2 = "provider2";

        var provider0 = A.Fake<IGitCredentialProvider>();
        var provider1 = A.Fake<IGitCredentialProvider>();

        A.CallTo(() => provider0.Name).Returns(providerName0);
        A.CallTo(() => provider1.Name).Returns(providerName1);

        var providers = new DefaultGitCredentialProviders(new[] { provider0, provider1 });

        // Act
        var provider = providers.GetProvider(providerName2);

        // Assert
        provider
            .Should()
            .BeNull();
    }

    [Fact]
    public void GetCredentials_UrlIsInFirstProvider_ReturnsCredentialsFromFirstProvider()
    {
        const string providerName0 = "provider0";
        const string providerName1 = "provider1";

        const string url = "https://git.domain.tld/repo.git";

        var provider0 = A.Fake<IGitCredentialProvider>();
        var provider1 = A.Fake<IGitCredentialProvider>();

        var expectedCredential = A.Fake<IGitCredential>();

        A.CallTo(() => provider0.GetCredentials(url, null)).Returns(expectedCredential);

        A.CallTo(() => provider0.Name).Returns(providerName0);
        A.CallTo(() => provider1.Name).Returns(providerName1);

        var providers = new DefaultGitCredentialProviders(new[] { provider0, provider1 });

        // Act
        var credential = providers.GetCredentials(url, null);

        // Assert
        credential
            .Should()
            .BeSameAs(expectedCredential);
    }
}