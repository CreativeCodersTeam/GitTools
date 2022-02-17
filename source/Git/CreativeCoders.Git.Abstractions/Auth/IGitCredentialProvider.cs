namespace CreativeCoders.Git.Abstractions.Auth
{
    /// <summary>   Interface for git credential provider. </summary>
    public interface IGitCredentialProvider
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the credentials for a specific git url. </summary>
        ///
        /// <param name="url">      URL of the resource. </param>
        /// <param name="fromUrl">  URL of from. </param>
        /// 
        ///
        /// <returns>   The credentials. </returns>
        ///-------------------------------------------------------------------------------------------------
        IGitCredential? GetCredentials(string url, string? fromUrl);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the name of the credential provider. </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------
        string Name { get; }
    }
}
