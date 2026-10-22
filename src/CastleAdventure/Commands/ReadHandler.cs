using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>READ: an item's <c>ReadText</c>, if it has any.</summary>
public sealed class ReadHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Read what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Character is not null)
        {
            return GameResponse.Say("You can't read that.");
        }

        if (target.Item is not { } item)
        {
            return GameResponse.Say(target.FailureMessage);
        }

        return GameResponse.Say(item.ReadText ?? $"There's nothing written on the {item.Name}.");
    }
}
