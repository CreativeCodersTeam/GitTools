﻿using System;
using System.Threading.Tasks;
using CreativeCoders.Core.IO;
using CreativeCoders.Core.SysEnvironment;
using CreativeCoders.GitTool.Base;
using CreativeCoders.SysConsole.App;
using CreativeCoders.SysConsole.Cli.Actions;
using Microsoft.Extensions.Configuration;

namespace CreativeCoders.GitTool.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await BuildApp(args).RunAsync();
    }

    private static IConsoleApp BuildApp(string[] args)
    {
        return new ConsoleAppBuilder(args)
            .UseActions<Startup>()
            .UseConfiguration(x =>
            {
                var toolConfigurationFile = FileSys.Path.Combine(
                    Env.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    GitToolApp.ConfigFolderName,
                    GitToolApp.ConfigFileName);

                if (FileSys.File.Exists(toolConfigurationFile))
                {
                    x.AddJsonFile(toolConfigurationFile);
                }
            })
            .Build();
    }
}
