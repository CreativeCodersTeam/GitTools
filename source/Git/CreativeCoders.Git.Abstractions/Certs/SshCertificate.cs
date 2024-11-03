using JetBrains.Annotations;

namespace CreativeCoders.Git.Abstractions.Certs;

[PublicAPI]
public class SshCertificate
{
    public SshCertificate(byte[] hashMd5, byte[] hashSha1, bool hasMd5, bool hasSha1)
    {
        HashMd5 = hashMd5;
        HashSha1 = hashSha1;
        HasMd5 = hasMd5;
        HasSha1 = hasSha1;
    }

    public byte[] HashMd5 { get; }

    public byte[] HashSha1 { get; }

    public bool HasMd5 { get; }

    public bool HasSha1 { get; }
}
