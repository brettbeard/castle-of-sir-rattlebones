using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>HINT: the first content hint that applies to the player's progress. Counted as a hint, not a move.</summary>
public sealed class HintHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        context.State.HintsUsed++;
        var hint = context.World.Hints.FirstOrDefault(h => h.Applies(context.State));
        return GameResponse.Say(hint?.Text ?? "You're doing fine. Keep exploring!") with { CountedMove = false };
    }
}
