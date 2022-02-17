namespace CreativeCoders.Git.Abstractions.Exceptions
{
    public class GitNoRemoteFoundException : GitException
    {
        public GitNoRemoteFoundException() : base("No remote can be found")
        {
        }
    }
}
