using CreativeCoders.Core.Text;
using CreativeCoders.Git.Abstractions.Auth;
using CreativeCoders.Git.Auth;
using FakeItEasy;
using FluentAssertions;
using LibGit2Sharp;
using Xunit;

namespace CreativeCoders.Git.UnitTests.Auth;

public class GitCredentialsHandlerTests
{
    [Fact]
    public void HandleCredentials_UserNamePasswordRequestedForExistingUrl_ReturnsCredentialsForUrl()
    {
        const string expectedUserName = "TestUser";
        const string expectedPassword = "TestPassword";

        const string url = "https://git.domain.tld/repo.git";

        var providers = A.Fake<IGitCredentialProviders>();

        var expectedCredential = new GitCredential(expectedUserName, expectedPassword);

        A.CallTo(() => providers.GetCredentials(url, null)).Returns(expectedCredential);

        var handler = new GitCredentialsHandler(providers);

        // Act
        var credentials =
            handler.HandleCredentials(url, null, SupportedCredentialTypes.UsernamePassword) as
                SecureUsernamePasswordCredentials;

        // Assert
        credentials
            .Should()
            .NotBeNull();

        credentials!.Username
            .Should()
            .Be(expectedUserName);

        credentials.Password.ToNormalString()
            .Should()
            .Be(expectedPassword);
    }

    [Fact]
    public void HandleCredentials_DefaultCredentialsRequested_ReturnsDefaultCredentials()
    {
        const string expectedUserName = "TestUser";
        const string expectedPassword = "TestPassword";

        const string url = "https://git.domain.tld/repo.git";

        var providers = A.Fake<IGitCredentialProviders>();

        var expectedCredential = new GitCredential(expectedUserName, expectedPassword);

        A.CallTo(() => providers.GetCredentials(url, null)).Returns(expectedCredential);

        var handler = new GitCredentialsHandler(providers);

        // Act
        var credentials =
            handler.HandleCredentials(url, null, SupportedCredentialTypes.Default) as
                DefaultCredentials;

        // Assert
        credentials
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void
        HandleCredentials_DefaultOrUserNameCredentialsRequestedNoProviderCredential_ReturnsDefaultCredentials()
    {
        const string url = "https://git.domain.tld/repo.git";

        var providers = A.Fake<IGitCredentialProviders>();

        A.CallTo(() => providers.GetCredentials(url, null)).Returns(null);

        var handler = new GitCredentialsHandler(providers);

        // Act
        var credentials =
            handler.HandleCredentials(
                    url, null,
                    SupportedCredentialTypes.UsernamePassword | SupportedCredentialTypes.Default)
                as DefaultCredentials;

        // Assert
        credentials
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void HandleCredentials_UserNameCredentialsRequestedNoProviderCredential_ReturnsNull()
    {
        const string url = "https://git.domain.tld/repo.git";

        var providers = A.Fake<IGitCredentialProviders>();

        A.CallTo(() => providers.GetCredentials(url, null)).Returns(null);

        var handler = new GitCredentialsHandler(providers);

        // Act
        var credentials =
            handler.HandleCredentials(
                url, null,
                SupportedCredentialTypes.UsernamePassword);

        // Assert
        credentials
            .Should()
            .BeNull();
    }
}