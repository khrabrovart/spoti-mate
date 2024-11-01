using SpotiMate.Cli;

namespace SpotiMate.Handlers;

public interface ICommandHandler
{
    Task<int> Handle(CliOptions options);
}
