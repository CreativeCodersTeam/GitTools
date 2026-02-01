using CreativeCoders.Cli.Hosting.Exceptions;

namespace CreativeCoders.GitTool.Cli.Commands.FeatureGroup.Finish;

public class FeatureFinishFailedException(string message, int exitCode, Exception? exception = null)
    : CliExitException(message, exitCode, exception);
