namespace CastleAdventure.Model;

public sealed class Room
{
    public required string Id { get; init; }

    /// <summary>Title-case name shown as the room heading, e.g. "Great Hall".</summary>
    public required string Name { get; init; }

    /// <summary>Full prose description. Mention scenery here; takeable items are listed separately.</summary>
    public required string Description { get; init; }

    /// <summary>State-dependent descriptions; the first that applies replaces <see cref="Description"/>.</summary>
    public List<ConditionalText> DescriptionWhen { get; } = [];

    public Dictionary<Direction, Exit> Exits { get; } = [];

    /// <summary>Items that start in this room. Where they are later is tracked in <see cref="GameState"/>.</summary>
    public List<Item> Items { get; } = [];

    /// <summary>Characters in this room. Characters never move.</summary>
    public List<Character> Characters { get; } = [];
}
