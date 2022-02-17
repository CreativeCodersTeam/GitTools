using System;

namespace CreativeCoders.Git.Abstractions.Exceptions
{
    public class GitLockedFileException : GitException
    {
        public GitLockedFileException(Exception innerException) : base("A file is locked", innerException)
        {
        }
    }
}
