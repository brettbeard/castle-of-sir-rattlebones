namespace CastleAdventure.Model;

/// <summary>Everything that changes during play. RESTART simply creates a new one.</summary>
public sealed class GameState
{
    private GameState(
        string currentRoomId,
        Dictionary<string, ItemLocation> itemLocations,
        Dictionary<string, string> characterStates)
    {
        CurrentRoomId = currentRoomId;
        ItemLocations = itemLocations;
        CharacterStates = characterStates;
    }

    public static GameState NewGame(World world) => new(
        world.StartRoomId,
        new Dictionary<string, ItemLocation>(world.InitialItemLocations),
        world.Characters.Values.ToDictionary(c => c.Id, c => c.InitialState));

    public string CurrentRoomId { get; set; }

    public Dictionary<string, ItemLocation> ItemLocations { get; }

    /// <summary>Current state of each character, keyed by character ID.</summary>
    public Dictionary<string, string> CharacterStates { get; }

    /// <summary>Hidden items the player has uncovered.</summary>
    public HashSet<string> RevealedItemIds { get; } = [];

    /// <summary>Locks are locked unless their ID is in here.</summary>
    public HashSet<string> UnlockedLockIds { get; } = [];

    public HashSet<string> VisitedRoomIds { get; } = [];

    public int Moves { get; set; }

    public int HintsUsed { get; set; }

    public bool HasWon { get; set; }

    public bool IsLocked(string lockId) => !UnlockedLockIds.Contains(lockId);

    public bool IsInInventory(string itemId) =>
        ItemLocations.TryGetValue(itemId, out var location) && location.Kind == LocationKind.Inventory;
}
