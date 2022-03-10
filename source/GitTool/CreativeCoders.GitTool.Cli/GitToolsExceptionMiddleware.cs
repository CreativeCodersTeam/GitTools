using System;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.SysConsole.Cli.Actions.Runtime;
using CreativeCoders.SysConsole.Cli.Actions.Runtime.Middleware;
using CreativeCoders.SysConsole.Core.Abstractions;

namespace CreativeCoders.GitTool.Cli;

public class GitToolsExceptionMiddleware : CliActionMiddlewareBase
{
    private readonly ISysConsole _sysConsole;

    public GitToolsExceptionMiddleware(Func<CliActionContext, Task> next, ISysConsole sysConsole)
        : base(next)
    {
        _sysConsole = Ensure.NotNull(sysConsole, nameof(sysConsole));
    }

    public override async Task InvokeAsync(CliActionContext context)
    {
        try
        {
            await Next(context);
        }
        catch (Exception e)
        {
            _sysConsole
                .WriteError(e.GetBaseException().Message)
                .WriteLine();

            context.ReturnCode = -1024;
        }
    }
}
