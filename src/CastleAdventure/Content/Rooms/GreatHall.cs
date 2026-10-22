using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class GreatHall
{
    public static Room Create() => new()
    {
        Id = RoomIds.GreatHall,
        Name = "Great Hall",
        Description =
            "The great hall stretches away into shadow. A long banquet table runs down its length, " +
            "still set for a feast nobody finished. Tattered banners hang from the rafters. " +
            "Archways lead west and east, the courtyard lies to the south, and a heavy oak door guards the way north.",
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
                Aliases = { "table", "feast", "plates", "goblets" },
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
                Id = ItemIds.OakDoor,
                Name = "oak door",
                Article = "an",
                Aliases = { "door", "heavy door", "heavy oak door", "north door", "lock", "keyhole" },
                Description = "Iron-banded oak, black with age. Its lock is engraved with a small silver crescent.",
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
