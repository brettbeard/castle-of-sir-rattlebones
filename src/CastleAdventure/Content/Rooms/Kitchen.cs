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
                Id = ItemIds.Trapdoor,
                Name = "trapdoor",
                Aliases = { "trap door", "steps", "stairs", "stone steps" },
                Description =
                    "A square of planks on rusty hinges, propped open. Stone steps lead down, and a draught " +
                    "drifts up carrying the smell of damp and, oddly, the sound of humming.",
                TakeRefusal = "It's hinged to the floor. You'd only end up taking the floor too.",
            },
        },
    };
}
