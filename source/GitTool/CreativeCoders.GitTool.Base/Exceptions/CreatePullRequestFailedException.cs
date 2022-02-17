using System;
using JetBrains.Annotations;

namespace CreativeCoders.GitTool.Base.Exceptions
{
    [PublicAPI]
    public class CreatePullRequestFailedException : Exception
    {
        public CreatePullRequestFailedException() : base("Create pull/merge request failed.")
        {
        }

        public CreatePullRequestFailedException(string message) : base(message)
        {
        }

        public CreatePullRequestFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
