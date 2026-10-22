using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Engine;

/// <summary>
/// Matches a noun phrase against the names and aliases of things in scope.
/// Failure messages distinguish words the game doesn't know at all from things that aren't here.
/// </summary>
public sealed class NounResolver
{
    private readonly HashSet<string> _knownWords;

    public NounResolver(World world)
    {
        var phrases = world.Items.Values.SelectMany(i => i.AllNames)
            .Concat(world.Characters.Values.SelectMany(c => c.AllNames))
            .Concat(world.Characters.Values.SelectMany(c => c.Topics.SelectMany(t => t.Keywords)))
            .Concat(Vocabulary.Directions.Keys)
            .Concat(Vocabulary.Prepositions.Keys);

        _knownWords = phrases.SelectMany(p => p.Split(' ')).ToHashSet();
    }

    public NounResolution Resolve(GameContext context, string noun, NounScope scope = NounScope.Everything)
    {
        if (scope == NounScope.Everything)
        {
            var character = context.PresentCharacters.FirstOrDefault(c => c.AllNames.Contains(noun));
            if (character is not null)
            {
                return NounResolution.Found(character);
            }
        }

        var candidates = scope == NounScope.Inventory
            ? context.Inventory
            : context.VisibleItemsIn(context.State.CurrentRoomId).Concat(context.Inventory);

        var matches = candidates.Where(i => i.AllNames.Contains(noun)).ToList();
        return matches.Count switch
        {
            1 => NounResolution.Found(matches[0]),
            > 1 => NounResolution.Failed(
                $"Which {noun} do you mean, {Prose.JoinOr(matches.Select(m => $"the {m.Name}"))}?", matches),
            _ => NounResolution.Failed(NotFoundMessage(noun)),
        };
    }

    /// <summary>True if every word in the phrase appears somewhere in the game's vocabulary.</summary>
    public bool IsKnown(string phrase) => FirstUnknownWord(phrase) is null;

    private string NotFoundMessage(string noun) =>
        FirstUnknownWord(noun) is { } unknown
            ? $"I don't know the word '{unknown}'."
            : $"You don't see any {noun} here.";

    private string? FirstUnknownWord(string phrase) =>
        phrase.Split(' ').FirstOrDefault(w => !_knownWords.Contains(w));
}
