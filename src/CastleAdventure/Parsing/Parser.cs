namespace CastleAdventure.Parsing;

/// <summary>
/// Turns a line of player input into a <see cref="ParsedCommand"/>. Pure and stateless:
/// it knows the words in <see cref="Vocabulary"/> but nothing about the world.
/// </summary>
/// <remarks>
/// Steps: lowercase → split on whitespace → trim punctuation → drop stopwords →
/// match the longest verb phrase (or a bare direction) → split the rest on the first preposition.
/// </remarks>
public static class Parser
{
    private static readonly char[] _trimChars = ['.', ',', '!', '?', ';', ':', '"', '(', ')', '[', ']'];

    public static ParsedCommand Parse(string? input)
    {
        var words = Tokenize(input ?? string.Empty);
        if (words.Count == 0)
        {
            return new ParsedCommand(Verb.None);
        }

        if (Vocabulary.Directions.TryGetValue(words[0], out var bareDirection))
        {
            return new ParsedCommand(Verb.Move) { Direction = bareDirection };
        }

        if (!TryMatchVerb(words, out var verb, out var verbLength))
        {
            return new ParsedCommand(Verb.Unknown) { UnknownWord = words[0] };
        }

        var rest = words.Skip(verbLength).ToList();

        if (verb == Verb.Move)
        {
            return ParseMove(rest);
        }

        // "look altar" means "examine altar"; plain "look" is a room look.
        if (verb == Verb.Look && rest.Count > 0)
        {
            verb = Verb.Examine;
        }

        return WithObjects(new ParsedCommand(verb), rest);
    }

    private static List<string> Tokenize(string input)
    {
        var words = new List<string>();
        foreach (var raw in input.ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var word = raw == "?" ? raw : raw.Trim(_trimChars);
            if (word.EndsWith("'s", StringComparison.Ordinal))
            {
                word = word[..^2];
            }

            word = word.Replace("'", string.Empty, StringComparison.Ordinal);

            if (word.Length > 0 && !Vocabulary.Stopwords.Contains(word))
            {
                words.Add(word);
            }
        }

        return words;
    }

    private static bool TryMatchVerb(List<string> words, out Verb verb, out int length)
    {
        for (length = Math.Min(Vocabulary.LongestVerbPhraseWords, words.Count); length > 0; length--)
        {
            var phrase = string.Join(' ', words.Take(length));
            if (Vocabulary.VerbPhrases.TryGetValue(phrase, out verb))
            {
                return true;
            }
        }

        verb = Verb.Unknown;
        return false;
    }

    private static ParsedCommand ParseMove(List<string> rest)
    {
        // "go north", "climb down the steps"; a missing direction is left for the engine to question.
        if (rest.Count > 0 && Vocabulary.Directions.TryGetValue(rest[0], out var direction))
        {
            return new ParsedCommand(Verb.Move) { Direction = direction };
        }

        return new ParsedCommand(Verb.Move) { DirectObject = JoinOrNull(rest) };
    }

    private static ParsedCommand WithObjects(ParsedCommand command, List<string> rest)
    {
        var prepIndex = rest.FindIndex(Vocabulary.Prepositions.ContainsKey);
        if (prepIndex < 0)
        {
            return command with { DirectObject = JoinOrNull(rest) };
        }

        return command with
        {
            DirectObject = JoinOrNull(rest.Take(prepIndex)),
            Preposition = Vocabulary.Prepositions[rest[prepIndex]],
            IndirectObject = JoinOrNull(rest.Skip(prepIndex + 1)),
        };
    }

    private static string? JoinOrNull(IEnumerable<string> words)
    {
        var joined = string.Join(' ', words);
        return joined.Length == 0 ? null : joined;
    }
}
