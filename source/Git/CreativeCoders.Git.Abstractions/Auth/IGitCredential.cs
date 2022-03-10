namespace CreativeCoders.Git.Abstractions.Auth;

/// <summary>   Interface representing git credential. </summary>
public interface IGitCredential
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the name of the user. </summary>
    ///
    /// <value> The name of the user. </value>
    ///-------------------------------------------------------------------------------------------------
    string UserName { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the password. </summary>
    ///
    /// <value> The password. </value>
    ///-------------------------------------------------------------------------------------------------
    string Password { get; }
}