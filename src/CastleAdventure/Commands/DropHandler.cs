using CastleAdventure.Engine;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>DROP: leaves a carried item in the current room, where it can be picked up again.</summary>
public sealed class DropHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Drop what?");
        }

        var carried = context.Nouns.Resolve(context, noun, NounScope.Inventory);
        if (carried.Item is { } item)
        {
            context.MoveItem(item, ItemLocation.InRoom(context.State.CurrentRoomId));
            return GameResponse.Say("Dropped.");
        }

        if (carried.Matches.Count > 1)
        {
            return GameResponse.Say(carried.FailureMessage);
        }

        // Not carried: say why, using the wider scope only to pick the right message.
        var anywhere = context.Nouns.Resolve(context, noun);
        return GameResponse.Say(anywhere.IsFound ? "You aren't carrying that." : anywhere.FailureMessage);
    }
}
