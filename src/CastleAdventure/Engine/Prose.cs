namespace CastleAdventure.Engine;

/// <summary>Small English helpers for generated text.</summary>
public static class Prose
{
    /// <summary>"a", "a and b", "a, b and c".</summary>
    public static string JoinAnd(IEnumerable<string> items) => Join(items, "and");

    /// <summary>"a", "a or b", "a, b or c".</summary>
    public static string JoinOr(IEnumerable<string> items) => Join(items, "or");

    private static string Join(IEnumerable<string> items, string conjunction)
    {
        var list = items.ToArray();
        return list.Length switch
        {
            0 => string.Empty,
            1 => list[0],
            _ => $"{string.Join(", ", list[..^1])} {conjunction} {list[^1]}",
        };
    }
}
