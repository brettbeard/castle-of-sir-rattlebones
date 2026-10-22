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
                Aliases = { "dry fountain", "basin", "stone fish", "fish", "leaves" },
                Description = "The basin is cracked and choked with dead leaves. The stone fish spouts nothing but cobwebs.",
                DescriptionWhen =
                {
                    new(s => s.RevealedItemIds.Contains(ItemIds.CopperCoin),
                        "The basin is cracked and choked with dead leaves. Nothing else glints among them."),
                },
                TakeRefusal = "You'd need a cart, six strong friends, and a very good reason.",
            },
            new Item
            {
                Id = ItemIds.Weeds,
                Name = "weeds",
                Article = "some",
                Aliases = { "weed", "grass", "nettles" },
                Description = "Nettles, thistles, and something that might once have been a rose garden. They hiss and sway in the wind.",
                TakeRefusal = "You pull up a handful of nettles, regret it immediately, and drop them.",
            },
            new Item
            {
                Id = ItemIds.GreatDoors,
                Name = "great doors",
                Article = "the",
                Aliases = { "doors", "gates", "gate", "castle doors" },
                Description = "Two enormous doors of weathered oak, hanging crooked on their hinges. Nobody has closed them in a very long time.",
                OpenedText = "They're already hanging open. Some invitations are hard to refuse.",
                TakeRefusal = "Each one weighs more than a horse.",
            },
            new Item
            {
                Id = ItemIds.Moon,
                Name = "moon",
                Article = "the",
                Aliases = { "sky", "stars", "moonlight" },
                Description = "A fat silver crescent hangs over the battlements, as if the castle had pinned its crest to the sky.",
                TakeRefusal = "You reach for it. It stays where it is.",
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
