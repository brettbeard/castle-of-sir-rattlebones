using CastleAdventure.Model;

namespace CastleAdventure.Engine;

/// <summary>
/// Shared lock logic for UNLOCK, OPEN, and USE. When no key is named, the carried key that fits
/// is chosen automatically. Unlocking an item that has <see cref="Item.OpenedText"/> also opens it.
/// </summary>
public static class LockActions
{
    /// <param name="target">An item with a <see cref="Item.Lock"/>.</param>
    /// <param name="keyNoun">The key the player named, or null to pick automatically.</param>
    /// <param name="opening">True for OPEN: an unlocked target is opened, and a missing key reads "It's locked."</param>
    public static GameResponse Unlock(GameContext context, Item target, string? keyNoun, bool opening)
    {
        var keyLock = target.Lock ?? throw new ArgumentException($"Item '{target.Id}' has no lock.", nameof(target));

        if (!context.State.IsLocked(keyLock.Id))
        {
            return opening ? Open(context, target) : GameResponse.Say(keyLock.AlreadyUnlockedMessage);
        }

        if (keyNoun is null)
        {
            var fitting = context.Inventory.FirstOrDefault(i => i.Id == keyLock.KeyId);
            if (fitting is null)
            {
                return GameResponse.Say(opening ? keyLock.LockedMessage : keyLock.NoKeyMessage);
            }
        }
        else if (ResolveKey(context, keyNoun, keyLock) is { } failure)
        {
            return failure;
        }

        context.State.UnlockedLockIds.Add(keyLock.Id);
        var unlocked = GameResponse.Say(keyLock.UnlockMessage);
        return target.OpenedText is null ? unlocked : Combine(unlocked, Open(context, target));
    }

    /// <summary>Opens an already-unlocked (or lockless) item.</summary>
    public static GameResponse Open(GameContext context, Item target)
    {
        if (target.OpenedText is null)
        {
            return GameResponse.Say(target.Lock is null ? "You can't open that." : "It's already open.");
        }

        if (target.IsWinTrigger)
        {
            context.State.HasWon = true;
        }

        return GameResponse.Say(target.OpenedText) with { Won = target.IsWinTrigger };
    }

    /// <summary>Checks the named key is carried and fits. Returns a failure response, or null when it fits.</summary>
    private static GameResponse? ResolveKey(GameContext context, string keyNoun, KeyLock keyLock)
    {
        var carried = context.Nouns.Resolve(context, keyNoun, NounScope.Inventory);

        // "unlock door with key" while holding both keys: prefer the one that fits.
        var key = carried.Item ?? carried.Matches.FirstOrDefault(m => m.Id == keyLock.KeyId);
        if (key is null)
        {
            if (carried.Matches.Count > 1)
            {
                return GameResponse.Say(carried.FailureMessage);
            }

            var anywhere = context.Nouns.Resolve(context, keyNoun);
            return GameResponse.Say(anywhere.IsFound ? "You aren't carrying that." : anywhere.FailureMessage);
        }

        if (key.Id == keyLock.KeyId)
        {
            return null;
        }

        return GameResponse.Say(keyLock.WrongKeyMessages.GetValueOrDefault(key.Id) ?? $"The {key.Name} doesn't fit this lock.");
    }

    private static GameResponse Combine(GameResponse first, GameResponse second) =>
        new([.. first.Blocks, .. second.Blocks]) { Won = second.Won };
}
