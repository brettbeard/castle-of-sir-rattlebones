using CastleAdventure.Engine;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>
/// USE Y [ON X]. The only use in this game is keys in locks: without a target, the locked thing in the
/// room is chosen; with "use key" while holding both keys, the one that fits is chosen.
/// </summary>
public sealed class UseHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } keyNoun)
        {
            return GameResponse.Say("Use what?");
        }

        Item? target;
        if (command.IndirectObject is { } targetNoun)
        {
            var resolved = context.Nouns.Resolve(context, targetNoun);
            if (!resolved.IsFound)
            {
                return GameResponse.Say(resolved.FailureMessage);
            }

            target = resolved.Item;
        }
        else
        {
            var lockable = context.VisibleItemsIn(context.State.CurrentRoomId).Where(i => i.Lock is not null).ToList();
            target = lockable.FirstOrDefault(i => i.Lock is { } l && context.State.IsLocked(l.Id)) ?? lockable.FirstOrDefault();
        }

        if (target?.Lock is null)
        {
            return IsAnyKey(context, keyNoun)
                ? GameResponse.Say("There's nothing here to use that on.")
                : GameResponse.Say("Nothing happens.");
        }

        return LockActions.Unlock(context, target, keyNoun, opening: false);
    }

    /// <summary>True if the phrase names a carried item that opens some lock in the world.</summary>
    private static bool IsAnyKey(GameContext context, string noun) =>
        context.Inventory.Any(i => i.AllNames.Contains(noun) && context.World.Locks.Values.Any(l => l.KeyId == i.Id));
}
