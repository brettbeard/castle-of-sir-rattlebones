using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>QUIT: asks <c>Program.cs</c> to end the game. Confirmation (Y/N) is the console's job.</summary>
public sealed class QuitHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context) =>
        new([]) { RequestQuit = true, CountedMove = false };
}
