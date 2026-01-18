using Cake.Common.Tools.InnoSetup;
using Cake.Frosting;
using CreativeCoders.CakeBuild.Tasks;
using JetBrains.Annotations;

namespace Build;

[UsedImplicitly]
[TaskName("CreateWin64Setup")]
public class CreateWin64SetupTask : FrostingTaskBase<BuildContext>
{
    protected override Task RunAsyncCore(BuildContext context)
    {
        context.InnoSetup(context.RootDir.CombineWithFilePath("setup/GitTool.iss"),
            new InnoSetupSettings()
            {
                Defines = { { "CiAppVersion", context.Version.FullSemVer } }
            });

        return Task.CompletedTask;
    }
}
