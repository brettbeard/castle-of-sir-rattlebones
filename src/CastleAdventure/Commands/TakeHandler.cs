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
            return GameResponse.Say(HeldItemRefusal(context, noun) ?? target.FailureMessage);
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

    /// <summary>"take key" near a character still holding a (hidden) key gets that character's refusal.</summary>
    private static string? HeldItemRefusal(GameContext context, string noun) =>
        context.PresentCharacters
            .Where(c => c.HeldItems.Any(i => i.AllNames.Contains(noun) && context.LocationOf(i) == ItemLocation.HeldBy(c.Id)))
            .Select(c => c.HeldItemTakeRefusal)
            .FirstOrDefault(r => r is not null);
}
