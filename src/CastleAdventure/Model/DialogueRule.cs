namespace CastleAdventure.Model;

/// <summary>
/// One possible reply from a character. The engine uses the first rule whose
/// <see cref="WhenStates"/> includes the character's current state.
/// </summary>
/// <param name="Reply">What the character says or does.</param>
public sealed record DialogueRule(string Reply)
{
    /// <summary>States in which this rule applies. Null means any state.</summary>
    public IReadOnlyList<string>? WhenStates { get; init; }

    /// <summary>If set, the character moves to this state after replying.</summary>
    public string? NewState { get; init; }

    /// <summary>If set, this item moves from the character to the player's inventory.</summary>
    public string? GivesItemId { get; init; }
}
