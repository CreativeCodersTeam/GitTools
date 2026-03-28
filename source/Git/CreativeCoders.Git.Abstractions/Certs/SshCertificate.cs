using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Certs;

/// <summary>
/// Represents an SSH host key certificate with MD5 and SHA-1 fingerprint hashes.
/// </summary>
[PublicAPI]
public class SshCertificate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SshCertificate"/> class.
    /// </summary>
    /// <param name="hashMd5">The MD5 hash of the host key.</param>
    /// <param name="hashSha1">The SHA-1 hash of the host key.</param>
    /// <param name="hasMd5"><see langword="true"/> if the MD5 hash is available; otherwise, <see langword="false"/>.</param>
    /// <param name="hasSha1"><see langword="true"/> if the SHA-1 hash is available; otherwise, <see langword="false"/>.</param>
    public SshCertificate(byte[] hashMd5, byte[] hashSha1, bool hasMd5, bool hasSha1)
    {
        HashMd5 = hashMd5;
        HashSha1 = hashSha1;
        HasMd5 = hasMd5;
        HasSha1 = hasSha1;
    }

    /// <summary>
    /// Gets the MD5 hash of the host key.
    /// </summary>
    public byte[] HashMd5 { get; }

    /// <summary>
    /// Gets the SHA-1 hash of the host key.
    /// </summary>
    public byte[] HashSha1 { get; }

    /// <summary>
    /// Gets a value that indicates whether the MD5 hash is available.
    /// </summary>
    /// <value><see langword="true"/> if the MD5 hash is available; otherwise, <see langword="false"/>.</value>
    public bool HasMd5 { get; }

    /// <summary>
    /// Gets a value that indicates whether the SHA-1 hash is available.
    /// </summary>
    /// <value><see langword="true"/> if the SHA-1 hash is available; otherwise, <see langword="false"/>.</value>
    public bool HasSha1 { get; }
}
