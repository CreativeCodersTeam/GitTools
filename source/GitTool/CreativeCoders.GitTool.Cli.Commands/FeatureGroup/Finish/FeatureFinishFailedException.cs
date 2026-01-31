using CreativeCoders.Cli.Hosting.Exceptions;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

public class FeatureFinishFailedException : CliExitException
{
    public FeatureFinishFailedException(string message, int exitCode, Exception? exception = null) : base(message,
        exitCode, exception)
    {
    }
}
