using CreativeCoders.CakeBuild;

namespace Build;

internal static class Program
{
    internal static int Main(string[] args)
    {
        return CakeHostBuilder.Create()
            .UseBuildContext<BuildContext>()
            .AddDefaultTasks()
            .AddTask<CreateWin64SetupTask>()
            .AddBuildServerIntegration()
            .InstallTools(
                new DotNetToolInstallation("GitVersion.Tool", "6.5.1"),
                new DotNetToolInstallation("dotnet-reportgenerator-globaltool", "5.5.1"))
            .Build()
            .Run(args);
    }
}
