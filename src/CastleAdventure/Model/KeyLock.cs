namespace CastleAdventure.Model;

/// <summary>
/// A lock on a door or container. Defined on the item that carries it (e.g. the oak door);
/// exits refer to it by <see cref="Id"/>. Whether it is currently locked lives in
/// <see cref="GameState.UnlockedLockIds"/>; every lock starts locked.
/// </summary>
public sealed class KeyLock
{
    public required string Id { get; init; }

    /// <summary>The item ID of the key that opens this lock.</summary>
    public required string KeyId { get; init; }

    /// <summary>Shown when the lock opens.</summary>
    public required string UnlockMessage { get; init; }

    /// <summary>Shown when the player tries to open it while locked and has no key that fits.</summary>
    public string LockedMessage { get; init; } = "It's locked.";

    /// <summary>Shown when the player tries to unlock it while holding no key that fits.</summary>
    public string NoKeyMessage { get; init; } = "You have nothing that fits this lock.";

    public string AlreadyUnlockedMessage { get; init; } = "It's already unlocked.";

    /// <summary>Custom messages when a specific wrong key is tried, keyed by that key's item ID.</summary>
    public Dictionary<string, string> WrongKeyMessages { get; } = [];
}
