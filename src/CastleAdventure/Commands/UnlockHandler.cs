using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>UNLOCK X [WITH Y]. Without a named key, the carried key that fits is used.</summary>
public sealed class UnlockHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Unlock what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Item is { Lock: not null } item)
        {
            return LockActions.Unlock(context, item, command.IndirectObject, opening: false);
        }

        return GameResponse.Say(target.IsFound ? "You can't unlock that." : target.FailureMessage);
    }
}
