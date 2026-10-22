using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>Handles one verb. Handlers read and change <see cref="GameContext.State"/>; they never touch the console.</summary>
public interface ICommandHandler
{
    GameResponse Handle(ParsedCommand command, GameContext context);
}
