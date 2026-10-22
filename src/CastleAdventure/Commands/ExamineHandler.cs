using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>EXAMINE / X / LOOK AT: describes an item, or a character according to its current state.</summary>
public sealed class ExamineHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Examine what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Character is { } character)
        {
            return GameResponse.Say(character.DescriptionByState[context.StateOf(character)]);
        }

        return target.Item is { } item
            ? GameResponse.Say(item.Description)
            : GameResponse.Say(target.FailureMessage);
    }
}
