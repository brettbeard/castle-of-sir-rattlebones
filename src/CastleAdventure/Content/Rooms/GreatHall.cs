using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class GreatHall
{
    private const string Opening =
        "The great hall stretches away into shadow. A long banquet table runs down its length, " +
        "still set for a feast nobody finished. Tattered banners hang from the rafters. " +
        "Archways lead west and east, and the courtyard lies to the south.";

    public static Room Create() => new()
    {
        Id = RoomIds.GreatHall,
        Name = "Great Hall",
        Description = $"{Opening} To the north, a heavy oak door stands firmly shut.",
        DescriptionWhen =
        {
            new(s => !s.IsLocked(LockIds.TreasuryDoor),
                $"{Opening} To the north, the heavy oak door stands open."),
        },
        Exits =
        {
            [Direction.South] = new Exit(RoomIds.Courtyard),
            [Direction.West] = new Exit(RoomIds.Kitchen),
            [Direction.East] = new Exit(RoomIds.Library),
            [Direction.North] = new Exit(RoomIds.Treasury)
            {
                LockId = LockIds.TreasuryDoor,
                BlockedMessage = "A heavy oak door bars the way north. Its lock is engraved with a silver crescent.",
            },
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.BanquetTable,
                Name = "banquet table",
                Aliases = { "table", "feast", "plates", "goblets", "pig", "centrepiece", "centerpiece" },
                Description =
                    "Pewter plates, toppled goblets, and a centrepiece that was probably a roast pig once. " +
                    "Everything wears a thick fur coat of dust.",
                TakeRefusal = "It's forty feet of solid oak. Perhaps just admire it.",
            },
            new Item
            {
                Id = ItemIds.Banners,
                Name = "tattered banners",
                Article = "some",
                Aliases = { "banners", "banner", "crest", "rafters" },
                Description = "Faded silver crescents on a midnight-blue field. The castle's crest, by the look of it.",
                TakeRefusal = "They're hung far too high, and they'd fall apart in your hands anyway.",
            },
            new Item
            {
                Id = ItemIds.Archways,
                Name = "archways",
                Article = "the",
                Aliases = { "archway", "arches", "arch" },
                Description = "Two stone archways: the one to the west smells faintly of old cooking, the one to the east of old paper.",
                TakeRefusal = "Archways are notoriously hard to carry.",
            },
            new Item
            {
                Id = ItemIds.OakDoor,
                Name = "oak door",
                Article = "an",
                Aliases = { "door", "heavy door", "heavy oak door", "north door", "lock", "keyhole" },
                Description = "Iron-banded oak, black with age. Its lock is engraved with a small silver crescent.",
                DescriptionWhen =
                {
                    new(s => !s.IsLocked(LockIds.TreasuryDoor),
                        "Iron-banded oak, black with age. It stands open, and beyond it lies a small vaulted chamber."),
                },
                TakeRefusal = "The door is firmly attached to the castle, which is firmly attached to the ground.",
                Lock = new KeyLock
                {
                    Id = LockIds.TreasuryDoor,
                    KeyId = ItemIds.SilverKey,
                    UnlockMessage = "The silver key turns stiffly in the lock. With a long groan, the oak door swings open.",
                    WrongKeyMessages =
                    {
                        [ItemIds.GoldKey] = "The gold key is far too small for this lock.",
                    },
                },
            },
        },
    };
}
