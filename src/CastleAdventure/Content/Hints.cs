using CastleAdventure.Model;

namespace CastleAdventure.Content;

/// <summary>HINT tiers in priority order (design.md §10). The first one that applies is shown.</summary>
public static class Hints
{
    public static IReadOnlyList<Hint> All { get; } =
    [
        new(s => NeedsSilverKey(s) && !s.IsInInventory(ItemIds.Diary),
            "The diary in the library might know where something shiny was hidden."),

        new(NeedsSilverKey,
            "Where do the faithful kneel? Look closely at what's there."),

        new(s => s.IsLocked(LockIds.TreasuryDoor),
            "That crescent-engraved door in the Great Hall looks promising."),

        new(s => !s.IsInInventory(ItemIds.GoldKey) && SkeletonIs(s, SkeletonStates.Unmet),
            "Have you looked beneath the kitchen?"),

        new(s => !s.IsInInventory(ItemIds.GoldKey) && SkeletonIs(s, SkeletonStates.Introduced),
            "Sir Rattlebones loves a chat. Have you asked about a key?"),

        new(s => !s.IsInInventory(ItemIds.GoldKey),
            "Sir Rattlebones gave you a gold key. Did you put it down somewhere?"),

        new(_ => true,
            "You have everything you need. To the chest!"),
    ];

    private static bool NeedsSilverKey(GameState s) =>
        !s.IsInInventory(ItemIds.SilverKey) && s.IsLocked(LockIds.TreasuryDoor);

    private static bool SkeletonIs(GameState s, string state) =>
        s.CharacterStates[CharacterIds.Skeleton] == state;
}
