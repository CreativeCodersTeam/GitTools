using FakeItEasy;
using FluentAssertions;
using Microsoft.Git.CredentialManager;
using Xunit;

namespace CreativeCoders.Git.Auth.CredentialManagerCore.UnitTests
{
    public class DefaultGcmCoreCredentialProviderTests
    {
        [Fact]
        public void Name_Get_CorrectName()
        {
            var gcmCoreCredentialStore = A.Fake<IGcmCoreCredentialStore>();

            var credentialManager = new DefaultGcmCoreCredentialProvider(gcmCoreCredentialStore);

            // Act and Assert
            credentialManager.Name
                .Should()
                .Be("GcmCore");
        }

        [Fact]
        public void GetCredential_ForKnownHost_ReturnsUserNameAndPassword()
        {
            const string expectedUserName = "TestUser";
            const string expectedPassword = "TestPassword";

            var credential = A.Fake<ICredential>();

            A.CallTo(() => credential.Account).Returns(expectedUserName);
            A.CallTo(() => credential.Password).Returns(expectedPassword);

            var credentialStore = A.Fake<ICredentialStore>();

            var gcmCoreCredentialStore = A.Fake<IGcmCoreCredentialStore>();

            A.CallTo(() => gcmCoreCredentialStore.Create("git")).Returns(credentialStore);

            A.CallTo(() => credentialStore.Get("https://git.git", null)).Returns(credential);

            var credentialManager = new DefaultGcmCoreCredentialProvider(gcmCoreCredentialStore);

            // Act
            var credentials = credentialManager.GetCredentials("https://git.git/repo.git", "");

            // Assert
            credentials
                .Should()
                .NotBeNull();

            credentials!.UserName
                .Should()
                .Be(expectedUserName);

            credentials.Password
                .Should()
                .Be(expectedPassword);
        }
    }
}
