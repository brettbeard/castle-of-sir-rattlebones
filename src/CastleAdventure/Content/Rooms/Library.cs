using CastleAdventure.Model;

namespace CastleAdventure.Content.Rooms;

public static class Library
{
    public static Room Create() => new()
    {
        Id = RoomIds.Library,
        Name = "Library",
        Description =
            "Bookshelves climb into the gloom, their crumbling spines furred with mould. " +
            "A reading desk sits beneath a narrow window, and the air smells of old paper and older secrets. " +
            "The only way out is west.",
        Exits =
        {
            [Direction.West] = new Exit(RoomIds.GreatHall),
        },
        Items =
        {
            new Item
            {
                Id = ItemIds.Diary,
                Name = "dusty diary",
                Aliases = { "diary", "journal" },
                Description =
                    "A small leather diary, its pages brittle and brown. The handwriting inside is cramped but legible. " +
                    "You could read it.",
                ReadText =
                    "Most of the entries complain about the cook. The last one reads: " +
                    "\"The steward grows greedy. I have hidden the silver key where the faithful kneel, " +
                    "and given the other to my old friend in the dungeon, who keeps everything under that ridiculous hat.\"",
                IsTakeable = true,
            },
            new Item
            {
                Id = ItemIds.Bookshelves,
                Name = "bookshelves",
                Article = "some",
                Aliases = { "shelves", "shelf", "bookshelf", "books", "book", "spines" },
                Description = "Hundreds of books, most of them too rotten to open.",
                RandomDetails =
                {
                    "One spine still reads: \"Bones and How to Keep Them\".",
                    "One spine still reads: \"101 Hat Tricks\".",
                    "One spine still reads: \"A Steward's Guide to Hiding Things (Badly)\".",
                    "One spine still reads: \"Moonhollow: A History, Volume XII\".",
                    "One spine still reads: \"Chains: A Love Story\".",
                    "One spine still reads: \"Dusting for Beginners\". It has never been opened.",
                },
                TakeRefusal = "The shelves are bolted to the wall, and the books would crumble at a touch.",
            },
        },
    };
}
