using CastleAdventure.Engine;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>
/// EXAMINE / X / LOOK AT: describes an item (and reveals anything it hides from an examining eye),
/// or a character according to its current state.
/// </summary>
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

        if (target.Item is not { } item)
        {
            return GameResponse.Say(target.FailureMessage);
        }

        var description = item.RandomDetails.Count == 0
            ? item.Description
            : $"{item.Description} {context.Flavor.Pick(item.RandomDetails, string.Empty)}";

        return GameResponse.Say([description, .. RevealActions.RevealFrom(context, item, RevealVerbs.Examine)]);
    }
}
