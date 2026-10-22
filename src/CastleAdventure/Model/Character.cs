namespace CastleAdventure.Model;

/// <summary>
/// A non-player character with a simple state machine. States are plain strings declared in
/// <see cref="States"/>; the current state lives in <see cref="GameState.CharacterStates"/>.
/// </summary>
public sealed class Character
{
    public required string Id { get; init; }

    /// <summary>Lowercase common name, e.g. "skeleton". Also matched as an alias.</summary>
    public required string Name { get; init; }

    public List<string> Aliases { get; } = [];

    public List<string> States { get; } = [];

    public required string InitialState { get; init; }

    /// <summary>Text appended to the room description, by state.</summary>
    public Dictionary<string, string> PresenceByState { get; } = [];

    /// <summary>Description for EXAMINE, by state.</summary>
    public Dictionary<string, string> DescriptionByState { get; } = [];

    /// <summary>Replies to TALK TO; first matching rule wins.</summary>
    public List<DialogueRule> Talk { get; } = [];

    public List<Topic> Topics { get; } = [];

    public required string UnknownTopicReply { get; init; }

    /// <summary>Reply when the player tries to take the character itself.</summary>
    public required string TakeRefusal { get; init; }

    /// <summary>Items the character starts with. Normally hidden until given away.</summary>
    public List<Item> HeldItems { get; } = [];

    /// <summary>Reply when the player tries to take an item the character is still holding.</summary>
    public string? HeldItemTakeRefusal { get; init; }

    public IEnumerable<string> AllNames => Aliases.Prepend(Name);
}
