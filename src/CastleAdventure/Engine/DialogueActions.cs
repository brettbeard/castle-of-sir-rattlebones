using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Engine;

/// <summary>Runs a character's <see cref="DialogueRule"/>s: picks the first rule for its current state and applies it.</summary>
public static class DialogueActions
{
    public static GameResponse Respond(GameContext context, Character character, IEnumerable<DialogueRule> rules, string fallback)
    {
        var state = context.StateOf(character);
        var rule = rules.FirstOrDefault(r => r.WhenStates is null || r.WhenStates.Contains(state));
        if (rule is null)
        {
            return GameResponse.Say(fallback);
        }

        if (rule.NewState is { } newState)
        {
            context.State.CharacterStates[character.Id] = newState;
        }

        if (rule.GivesItemId is { } itemId)
        {
            context.MoveItem(context.World.Items[itemId], ItemLocation.Inventory);
            context.State.RevealedItemIds.Add(itemId);
        }

        return GameResponse.Say(context.Flavor.Pick([rule.Reply, .. rule.Alternatives], rule.Reply));
    }

    /// <summary>The character the player means: the named one, or the only one present when none is named.</summary>
    public static Character? FindListener(GameContext context, string? noun, out string failure)
    {
        failure = string.Empty;
        if (noun is null || Vocabulary.CharacterPronouns.Contains(noun))
        {
            var present = context.PresentCharacters.ToList();
            if (present.Count == 1)
            {
                return present[0];
            }

            failure = present.Count == 0 ? "There's nobody here to talk to." : "Who do you mean?";
            return null;
        }

        var target = context.Nouns.Resolve(context, noun);
        if (target.Character is { } character)
        {
            return character;
        }

        failure = target.Item is { } item ? $"The {item.Name} isn't much of a conversationalist." : target.FailureMessage;
        return null;
    }
}
