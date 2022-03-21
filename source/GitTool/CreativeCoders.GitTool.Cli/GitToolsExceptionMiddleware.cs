using System;
using System.Threading.Tasks;
using CreativeCoders.Core;
using CreativeCoders.Core.Collections;
using CreativeCoders.GitTool.Base.Output;
using CreativeCoders.SysConsole.Cli.Actions.Exceptions;
using CreativeCoders.SysConsole.Cli.Actions.Runtime;
using CreativeCoders.SysConsole.Cli.Actions.Runtime.Middleware;
using CreativeCoders.SysConsole.Core.Abstractions;
using Spectre.Console;

namespace CreativeCoders.GitTool.Cli;

public class GitToolsExceptionMiddleware : CliActionMiddlewareBase
{
    private readonly ICml _cml;

    private readonly IAnsiConsole _ansiConsole;

    public GitToolsExceptionMiddleware(Func<CliActionContext, Task> next,
        IAnsiConsole ansiConsole, ICml cml)
        : base(next)
    {
        _cml = Ensure.NotNull(cml, nameof(cml));
        _ansiConsole = Ensure.NotNull(ansiConsole, nameof(ansiConsole));
    }

    public override async Task InvokeAsync(CliActionContext context)
    {
        try
        {
            await Next(context);
        }
        catch (AmbiguousRouteException e)
        {
            _ansiConsole.WriteMarkupLine(_cml.Error($"Unknown command '{string.Join(' ', e.Arguments)}'"));

            e.Routes.ForEach(x => _ansiConsole.WriteMarkupLine(string.Join(' ', x.RouteParts)));
        }
        catch (Exception e)
        {
            _ansiConsole
                .WriteMarkupLine(e.GetBaseException().Message)
                .WriteLine();

            context.ReturnCode = -1024;
        }
    }
}
