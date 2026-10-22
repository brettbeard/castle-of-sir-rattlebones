using CastleAdventure.Model;

namespace CastleAdventure.Engine;

/// <summary>
/// The outcome of resolving a noun phrase. Exactly one of <see cref="Item"/> / <see cref="Character"/>
/// is set on success; on failure <see cref="FailureMessage"/> is ready to show the player.
/// </summary>
public sealed record NounResolution
{
    public Item? Item { get; init; }

    public Character? Character { get; init; }

    /// <summary>Empty when found.</summary>
    public string FailureMessage { get; init; } = string.Empty;

    /// <summary>Every in-scope item that matched (more than one when ambiguous).</summary>
    public IReadOnlyList<Item> Matches { get; init; } = [];

    public bool IsFound => Item is not null || Character is not null;

    public static NounResolution Found(Item item) => new() { Item = item, Matches = [item] };

    public static NounResolution Found(Character character) => new() { Character = character };

    public static NounResolution Failed(string message, IReadOnlyList<Item>? matches = null) =>
        new() { FailureMessage = message, Matches = matches ?? [] };
}
