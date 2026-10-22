using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>RESTART: asks <c>Program.cs</c> to confirm and start a fresh game. Does not count as a move.</summary>
public sealed class RestartHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context) =>
        new([]) { RequestRestart = true, CountedMove = false };
}
