namespace CastleAdventure.Model;

/// <summary>A way out of a room. Every exit must have a matching return exit in the target room.</summary>
public sealed record Exit(string TargetRoomId)
{
    /// <summary>If set, the exit is blocked until this lock is unlocked.</summary>
    public string? LockId { get; init; }

    /// <summary>Shown when the player tries to use the exit while it is locked.</summary>
    public string? BlockedMessage { get; init; }
}
