using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Courtyard
{
    public static Room Create() => new()
    {
        Id = RoomIds.Courtyard,
        Name = "Courtyard",
        Description =
            "You stand in an overgrown courtyard, knee-deep in weeds that hiss in the wind. " +
            "A dry fountain squats in the middle, its stone fish gaping at the moon. " +
            "The castle's great doors hang open to the north, and a small chapel huddles against the wall to the east.",
        Exits =
        {
            [Direction.North] = new Exit(RoomIds.GreatHall),
            [Direction.East] = new Exit(RoomIds.Chapel),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.Fountain,
                Name = "fountain",
                Aliases = { "dry fountain", "basin", "stone fish", "fish" },
                Description = "The basin is cracked and choked with dead leaves. The stone fish spouts nothing but cobwebs.",
                TakeRefusal = "You'd need a cart, six strong friends, and a very good reason.",
            },
            new Item
            {
                Id = ItemIds.CopperCoin,
                Name = "copper coin",
                Aliases = { "coin", "copper" },
                Description = "A worn copper coin. One side shows a crescent moon; the other, a face rubbed away to nothing.",
                IsTakeable = true,
                IsHidden = true,
                RevealedBy = new RevealTrigger(
                    ItemIds.Fountain,
                    RevealVerbs.Examine | RevealVerbs.Search,
                    "Among the leaves at the bottom of the basin, something glints: a worn copper coin."),
            },
        },
    };
}
