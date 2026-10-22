using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Treasury
{
    public static Room Create() => new()
    {
        Id = RoomIds.Treasury,
        Name = "Treasury",
        Description =
            "A small vaulted chamber, its walls lined with empty shelves where riches once sat. " +
            "In the middle of the floor rests an iron-bound treasure chest with a tiny golden lock. " +
            "A suit of armour stands guard in the corner. The great hall is back to the south.",
        Exits =
        {
            [Direction.South] = new Exit(RoomIds.GreatHall),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.TreasureChest,
                Name = "treasure chest",
                Aliases = { "chest", "box", "golden lock", "tiny lock", "lock" },
                Description = "Iron-bound oak, studded with brass. The lock is small and golden.",
                TakeRefusal = "It's far too heavy to lift.",
                IsWinTrigger = true,
                OpenedText =
                    "You heave back the lid, and golden light spills across your face: coins, jewels, a jewelled crown, " +
                    "and, tucked neatly in one corner, a spare feathered hat. Sir Rattlebones, it seems, plans ahead.",
                Lock = new KeyLock
                {
                    Id = LockIds.Chest,
                    KeyId = ItemIds.GoldKey,
                    UnlockMessage = "The gold key turns with a satisfying click.",
                    WrongKeyMessages =
                    {
                        [ItemIds.SilverKey] = "The silver key doesn't fit this tiny golden lock.",
                    },
                },
            },
            new Item
            {
                Id = ItemIds.SuitOfArmor,
                Name = "suit of armour",
                Aliases = { "armour", "armor", "suit", "suit of armor" },
                Description = "An empty suit of plate armour. As you look at it, it creaks ominously. Then it does nothing at all.",
                TakeRefusal = "It's bolted to its stand. Probably for the best.",
            },
        },
    };
}
