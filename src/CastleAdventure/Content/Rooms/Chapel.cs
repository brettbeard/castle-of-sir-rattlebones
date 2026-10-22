using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Chapel
{
    public static Room Create() => new()
    {
        Id = RoomIds.Chapel,
        Name = "Chapel",
        Description =
            "A small chapel, cold as a tomb. Moonlight pours through a stained glass window, " +
            "laying coloured shapes across the flagstones. At the far end stands a cracked stone altar " +
            "draped in a moth-eaten cloth. The courtyard is back to the west.",
        Exits =
        {
            [Direction.West] = new Exit(RoomIds.Courtyard),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.Altar,
                Name = "altar",
                Article = "an",
                Aliases = { "stone altar", "cracked altar" },
                Description = "A slab of cracked grey stone. Something beneath the cloth catches the moonlight.",
                DescriptionWhen =
                {
                    new(s => s.RevealedItemIds.Contains(ItemIds.SilverKey),
                        "A slab of cracked grey stone, its cloth pushed aside. Whatever it was hiding, you've found it."),
                },
                TakeRefusal = "Even with a team of oxen, no.",
            },
            new Item
            {
                Id = ItemIds.AltarCloth,
                Name = "altar cloth",
                Article = "an",
                Aliases = { "cloth", "moth-eaten cloth" },
                Description = "A moth-eaten altar cloth, once white, now the colour of weak tea.",
                TakeRefusal = "It's so rotten it would fall apart. Better to leave it where it is.",
            },
            new Item
            {
                Id = ItemIds.StainedGlassWindow,
                Name = "stained glass window",
                Aliases = { "window", "stained glass", "glass", "knight", "shapes" },
                Description = "A knight in silver armour kneels before a golden chest. Subtle, whoever designed this.",
                TakeRefusal = "It's set in solid stone, and it's rather beautiful where it is.",
            },
            new Item
            {
                Id = ItemIds.Flagstones,
                Name = "flagstones",
                Article = "the",
                Aliases = { "floor", "flagstone", "stones" },
                Description = "Worn smooth by centuries of kneeling. The hollows are deepest right in front of the altar.",
                TakeRefusal = "They're mortared in tight. The castle would like to keep its floor.",
            },
            new Item
            {
                Id = ItemIds.SilverKey,
                Name = "silver key",
                Aliases = { "key", "silver" },
                Description = "A slender silver key. Its bow is shaped like a crescent moon.",
                IsTakeable = true,
                IsHidden = true,
                RevealedBy = new RevealTrigger(
                    ItemIds.AltarCloth,
                    RevealVerbs.Examine | RevealVerbs.Search,
                    "You lift the edge of the cloth. Beneath it, tucked against the stone, lies a silver key!"),
            },
        },
    };
}
