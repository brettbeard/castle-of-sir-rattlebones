using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Kitchen
{
    public static Room Create() => new()
    {
        Id = RoomIds.Kitchen,
        Name = "Kitchen",
        Description =
            "Cold ashes fill a hearth big enough to roast an ox, and something small skitters out of sight as you enter. " +
            "In the floor, a trapdoor stands open above a flight of stone steps leading down into darkness. " +
            "The great hall is back to the east.",
        Exits =
        {
            [Direction.East] = new Exit(RoomIds.GreatHall),
            [Direction.Down] = new Exit(RoomIds.Dungeon),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.RustyPot,
                Name = "rusty pot",
                Aliases = { "pot" },
                Description = "Rust has eaten most of it. It is useful for nothing except, perhaps, more rust.",
                IsTakeable = true,
            },
            new Item
            {
                Id = ItemIds.Hearth,
                Name = "hearth",
                Aliases = { "fireplace", "ashes", "ash", "fire" },
                Description = "A cavern of soot-blackened stone. The ashes are long cold, and something has been nesting in them.",
                TakeRefusal = "You get a handful of cold ash and a strong urge to wash your hands.",
            },
            new Item
            {
                Id = ItemIds.Trapdoor,
                Name = "trapdoor",
                Aliases = { "trap door", "steps", "stairs", "stone steps" },
                Description =
                    "A square of planks on rusty hinges, propped open. Stone steps lead down, and a draught " +
                    "drifts up carrying the smell of damp and, oddly, the sound of humming.",
                OpenedText = "It's already open, propped on its rusty hinges. The steps lead down.",
                TakeRefusal = "It's hinged to the floor. You'd only end up taking the floor too.",
            },
        },
    };
}
