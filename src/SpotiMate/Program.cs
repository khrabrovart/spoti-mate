using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using SpotiMate.Cli;
using SpotiMate.Handlers;

namespace SpotiMate;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            return await Parser.Default
                .ParseArguments<RunAllOptions, SearchTracksOptions>(args)
                .MapResult(
                    (CliOptions options) =>
                    {
                        var services = Bootstrapper.Bootstrap(options);
                        var handler = services.GetRequiredService<ICommandHandler>();

                        return handler.Handle(options);
                    },
                    _ => Task.FromResult(1));
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.ToString());
            return 1;
        }
    }
}
