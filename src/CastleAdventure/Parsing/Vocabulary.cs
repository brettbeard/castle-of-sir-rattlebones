using CastleAdventure.Model;

namespace CastleAdventure.Parsing;

/// <summary>
/// Every word the parser understands that isn't a world noun. Add synonyms here.
/// All entries are lowercase; multi-word verb phrases are matched longest first.
/// </summary>
public static class Vocabulary
{
    /// <summary>Words dropped before parsing.</summary>
    public static IReadOnlySet<string> Stopwords { get; } = new HashSet<string>
    {
        "the", "a", "an", "some", "my",
    };

    public static IReadOnlyDictionary<string, Verb> VerbPhrases { get; } = new Dictionary<string, Verb>
    {
        ["go"] = Verb.Move,
        ["walk"] = Verb.Move,
        ["climb"] = Verb.Move,

        ["look"] = Verb.Look,
        ["l"] = Verb.Look,
        ["look around"] = Verb.Look,

        ["examine"] = Verb.Examine,
        ["x"] = Verb.Examine,
        ["inspect"] = Verb.Examine,
        ["look at"] = Verb.Examine,

        ["read"] = Verb.Read,

        ["take"] = Verb.Take,
        ["get"] = Verb.Take,
        ["grab"] = Verb.Take,
        ["pick up"] = Verb.Take,

        ["drop"] = Verb.Drop,
        ["discard"] = Verb.Drop,
        ["put down"] = Verb.Drop,

        ["inventory"] = Verb.Inventory,
        ["inv"] = Verb.Inventory,
        ["i"] = Verb.Inventory,

        ["open"] = Verb.Open,
        ["unlock"] = Verb.Unlock,
        ["use"] = Verb.Use,

        ["lift"] = Verb.Search,
        ["move"] = Verb.Search,
        ["raise"] = Verb.Search,
        ["search"] = Verb.Search,
        ["look under"] = Verb.Search,
        ["look behind"] = Verb.Search,
        ["look in"] = Verb.Search,

        ["talk"] = Verb.Talk,
        ["talk to"] = Verb.Talk,
        ["talk with"] = Verb.Talk,
        ["speak"] = Verb.Talk,
        ["speak to"] = Verb.Talk,
        ["speak with"] = Verb.Talk,
        ["greet"] = Verb.Talk,

        ["ask"] = Verb.Ask,

        ["help"] = Verb.Help,
        ["?"] = Verb.Help,
        ["hint"] = Verb.Hint,
        ["hints"] = Verb.Hint,
        ["restart"] = Verb.Restart,
        ["quit"] = Verb.Quit,
        ["q"] = Verb.Quit,
        ["exit"] = Verb.Quit,
    };

    public static IReadOnlyDictionary<string, Direction> Directions { get; } = new Dictionary<string, Direction>
    {
        ["north"] = Direction.North,
        ["n"] = Direction.North,
        ["south"] = Direction.South,
        ["s"] = Direction.South,
        ["east"] = Direction.East,
        ["e"] = Direction.East,
        ["west"] = Direction.West,
        ["w"] = Direction.West,
        ["up"] = Direction.Up,
        ["u"] = Direction.Up,
        ["down"] = Direction.Down,
        ["d"] = Direction.Down,
    };

    /// <summary>Prepositions that split a command into direct and indirect objects, mapped to canonical form.</summary>
    public static IReadOnlyDictionary<string, string> Prepositions { get; } = new Dictionary<string, string>
    {
        ["with"] = "with",
        ["using"] = "with",
        ["on"] = "on",
        ["onto"] = "on",
        ["to"] = "to",
        ["about"] = "about",
        ["at"] = "at",
        ["in"] = "in",
        ["into"] = "in",
    };

    public static int LongestVerbPhraseWords { get; } = VerbPhrases.Keys.Max(k => k.Split(' ').Length);
}
