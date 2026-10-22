namespace CastleAdventure.Model;

/// <summary>
/// Anything the player can refer to: takeable objects, scenery, doors, containers.
/// Takeable items are listed in room descriptions; scenery should be mentioned in the room's prose.
/// </summary>
public sealed class Item
{
    public required string Id { get; init; }

    /// <summary>Lowercase display name, e.g. "rusty pot". Also matched as an alias.</summary>
    public required string Name { get; init; }

    public string Article { get; init; } = "a";

    /// <summary>Extra lowercase phrases the player can use for this item, e.g. "pot".</summary>
    public List<string> Aliases { get; } = [];

    public required string Description { get; init; }

    /// <summary>Text for READ. When null, READ says there's nothing written on it.</summary>
    public string? ReadText { get; init; }

    public bool IsTakeable { get; init; }

    /// <summary>Custom reply when the player tries to take a non-takeable item.</summary>
    public string? TakeRefusal { get; init; }

    /// <summary>Hidden items are out of scope until revealed (or given to the player).</summary>
    public bool IsHidden { get; init; }

    public RevealTrigger? RevealedBy { get; init; }

    /// <summary>A lock carried by this item (a door or container).</summary>
    public KeyLock? Lock { get; init; }

    /// <summary>Shown when the item is successfully opened.</summary>
    public string? OpenedText { get; init; }

    /// <summary>Opening this item wins the game.</summary>
    public bool IsWinTrigger { get; init; }

    /// <summary>Name with article, e.g. "a rusty pot".</summary>
    public string ListName => $"{Article} {Name}";

    public IEnumerable<string> AllNames => Aliases.Prepend(Name);
}
