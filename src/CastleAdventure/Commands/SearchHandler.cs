using CastleAdventure.Engine;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>LIFT / MOVE / SEARCH / LOOK UNDER: reveals items hidden beneath or inside something.</summary>
public sealed class SearchHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.DirectObject is not { } noun)
        {
            return GameResponse.Say("Search what?");
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Character is not null)
        {
            return GameResponse.Say("It would rather you kept your hands to yourself.");
        }

        if (target.Item is not { } item)
        {
            return GameResponse.Say(target.FailureMessage);
        }

        var found = RevealActions.RevealFrom(context, item, RevealVerbs.Search);
        return found.Count > 0
            ? GameResponse.Say([.. found])
            : GameResponse.Say($"You search the {item.Name} but find nothing of interest.");
    }
}
