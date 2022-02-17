using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Auth
{
    /// <summary>   Interface for collection of git credential providers. </summary>
    [PublicAPI]
    public interface IGitCredentialProviders : IGitCredentialProvider
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a provider for <paramref name="providerName"/>. </summary>
        ///
        /// <param name="providerName"> Name of the provider. </param>
        ///
        /// <returns>   The provider. </returns>
        ///-------------------------------------------------------------------------------------------------
        IGitCredentialProvider? GetProvider(string providerName);
    }
}
