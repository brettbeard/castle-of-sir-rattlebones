using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>HELP / ?: the reference card from content. Does not count as a move.</summary>
public sealed class HelpHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context) =>
        GameResponse.Say(context.World.HelpText) with { CountedMove = false };
}
