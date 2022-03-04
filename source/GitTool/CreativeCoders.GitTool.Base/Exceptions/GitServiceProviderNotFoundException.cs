using System;

namespace CreativeCoders.GitTool.Base.Exceptions;

public class GitServiceProviderNotFoundException : Exception
{
    public GitServiceProviderNotFoundException() : base("No git service provider found")
    {
        
    }
}