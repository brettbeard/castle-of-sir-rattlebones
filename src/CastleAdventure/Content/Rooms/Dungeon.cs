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
                Aliases = { "hat", "feather", "plume", "skeleton hat" },
                Description =
                    "A wide-brimmed hat with a single bedraggled plume, perched on the skeleton's skull at a rakish angle. " +
                    "The crown bulges slightly, as if something were tucked beneath it.",
                DescriptionWhen =
                {
                    new(s => s.CharacterStates[CharacterIds.Skeleton] == SkeletonStates.KeyGiven,
                        "A wide-brimmed hat with a single bedraggled plume, perched on Sir Rattlebones' skull at a rakish angle. " +
                        "The crown sits a little flatter now."),
                },
                TakeRefusal = "\"Hands off the hat, friend! A gentleman is nothing without his hat.\"",
            },
            new Item
            {
                Id = ItemIds.Straw,
                Name = "straw",
                Article = "some",
                Aliases = { "rotting straw", "floor", "water", "puddle" },
                Description = "Damp, black, and older than anyone would like. Water drips into it from somewhere overhead.",
                TakeRefusal = "You'd rather not. It squelches.",
            },
            new Item
            {
                Id = ItemIds.DungeonSteps,
                Name = "stone steps",
                Article = "the",
                Aliases = { "steps", "stairs", "stair" },
                Description = "Worn stone steps lead up towards the kitchen and a faint square of light.",
                TakeRefusal = "They're carved out of the castle itself.",
            },
        },
        Characters =
        {
            Skeleton.Create(),
        },
    };
}
