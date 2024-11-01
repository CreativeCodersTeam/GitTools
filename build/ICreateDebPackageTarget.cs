using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Nuke.Common;
using Nuke.Common.IO;

namespace DefaultNamespace;

public interface ICreateDebPackageTarget
{
    Target CreateDebPackage => d => d
        .OnlyWhenDynamic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        .Executes(() =>
        {
            InstallSystemRequirements();

            CreatePackageStructure();

            BuildDebPackage();
        });

    string DebPackageDefinition { get; }

    string DistPath { get; }

    string AppName { get; }

    void BuildDebPackage()
    {
        Process.Start("sh", "dpkg-deb --build gittool");
    }

    void InstallSystemRequirements()
    {
        //Process.Start("sh", "sudo apt-get install ")
    }

    void CreatePackageStructure()
    {
        var tmpPath = (AbsolutePath)Path.GetTempPath();

        Directory.CreateDirectory(tmpPath / AppName / "DEBIAN");

        //var controlFile = File.ReadAllText("")
    }
}
