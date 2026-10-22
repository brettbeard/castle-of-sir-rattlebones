namespace CastleAdventure.Content;

// All content IDs in one place, so a single search finds every use.
// (Deliberate exception to the one-type-per-file rule.)

public static class RoomIds
{
    public const string Courtyard = "courtyard";
    public const string GreatHall = "great_hall";
    public const string Kitchen = "kitchen";
    public const string Library = "library";
    public const string Chapel = "chapel";
    public const string Dungeon = "dungeon";
    public const string Treasury = "treasury";
}

public static class ItemIds
{
    // Courtyard
    public const string Fountain = "fountain";
    public const string CopperCoin = "copper_coin";

    // Great Hall
    public const string BanquetTable = "banquet_table";
    public const string Banners = "banners";
    public const string OakDoor = "oak_door";

    // Kitchen
    public const string RustyPot = "rusty_pot";
    public const string Trapdoor = "trapdoor";

    // Library
    public const string Diary = "diary";
    public const string Bookshelves = "bookshelves";

    // Chapel
    public const string Altar = "altar";
    public const string AltarCloth = "altar_cloth";
    public const string StainedGlassWindow = "stained_glass_window";
    public const string SilverKey = "silver_key";

    // Dungeon
    public const string Chains = "chains";
    public const string FeatheredHat = "feathered_hat";
    public const string GoldKey = "gold_key";

    // Treasury
    public const string TreasureChest = "treasure_chest";
    public const string SuitOfArmor = "suit_of_armor";
}

public static class LockIds
{
    public const string TreasuryDoor = "treasury_door_lock";
    public const string Chest = "chest_lock";
}

public static class CharacterIds
{
    public const string Skeleton = "skeleton";
}

public static class SkeletonStates
{
    public const string Unmet = "unmet";
    public const string Introduced = "introduced";
    public const string KeyGiven = "key_given";
}

public static class TopicIds
{
    public const string Key = "key";
    public const string SilverKey = "silver_key";
    public const string Hat = "hat";
    public const string Castle = "castle";
    public const string Treasure = "treasure";
    public const string Chest = "chest";
    public const string Chains = "chains";
    public const string Diary = "diary";
    public const string Coin = "coin";
    public const string Self = "self";
}
