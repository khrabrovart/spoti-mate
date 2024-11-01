using CommandLine;
using SpotiMate.Cli;

namespace SpotiMate;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            return await Parser.Default
                .ParseArguments<FindDuplicatesOptions, SynchronizeArtistsOptions, SearchTracksOptions>(args)
                .MapResult(
                    (CliOptions options) => new CommandHandler().Handle(options),
                    _ => Task.FromResult(1));
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.ToString());
            return 1;
        }
    }
}
