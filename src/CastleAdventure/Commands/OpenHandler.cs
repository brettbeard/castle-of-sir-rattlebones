using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>OPEN X [WITH Y]. A locked target is unlocked and opened in one step if the player has the key.</summary>
public sealed class OpenHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Open what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Item is not { } item)
        {
            return GameResponse.Say(target.IsFound ? "You can't open that." : target.FailureMessage);
        }

        return item.Lock is null
            ? LockActions.Open(context, item)
            : LockActions.Unlock(context, item, command.IndirectObject, opening: true);
    }
}
