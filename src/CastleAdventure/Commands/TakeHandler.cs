using CastleAdventure.Engine;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>TAKE / GET / PICK UP: moves a takeable item into the inventory; scenery and characters refuse in character.</summary>
public sealed class TakeHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Take what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Character is { } character)
        {
            return GameResponse.Say(character.TakeRefusal);
        }

        if (target.Item is not { } item)
        {
            return GameResponse.Say(target.FailureMessage);
        }

        if (context.LocationOf(item).Kind == LocationKind.Inventory)
        {
            return GameResponse.Say("You already have that.");
        }

        if (!item.IsTakeable)
        {
            return GameResponse.Say(item.TakeRefusal ?? "You can't take that.");
        }

        context.MoveItem(item, ItemLocation.Inventory);
        return GameResponse.Say("Taken.");
    }
}
