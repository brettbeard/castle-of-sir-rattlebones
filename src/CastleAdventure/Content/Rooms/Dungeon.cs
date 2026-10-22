using CastleAdventure.Content.Characters;
using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Dungeon
{
    public static Room Create() => new()
    {
        Id = RoomIds.Dungeon,
        Name = "Dungeon",
        Description =
            "Water drips somewhere in the dark. Rotting straw covers the floor, and rusted chains hang from the walls. " +
            "Stone steps lead up to the kitchen.",
        Exits =
        {
            [Direction.Up] = new Exit(RoomIds.Kitchen),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.Chains,
                Name = "chains",
                Article = "some",
                Aliases = { "chain", "shackles", "shackle" },
                Description =
                    "Rusted solid. Whoever is chained here isn't going anywhere, and judging by the dust, " +
                    "hasn't for a very long while.",
                TakeRefusal = "They're rusted to the wall, and to the skeleton.",
            },
            new Item
            {
                Id = ItemIds.FeatheredHat,
                Name = "feathered hat",
                Aliases = { "hat", "feather", "plume" },
                Description =
                    "A wide-brimmed hat with a single bedraggled plume, perched on the skeleton's skull at a rakish angle. " +
                    "The crown bulges slightly, as if something were tucked beneath it.",
                TakeRefusal = "\"Hands off the hat, friend! A gentleman is nothing without his hat.\"",
            },
        },
        Characters =
        {
            Skeleton.Create(),
        },
    };
}
