using CastleAdventure.Content;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class LockTests
{
    // ---- Treasury door ----

    [TestMethod]
    [DataRow("unlock door")]
    [DataRow("unlock the oak door with the silver key")]
    [DataRow("unlock door with key")]
    [DataRow("open door")]
    [DataRow("use silver key")]
    [DataRow("use key")]
    [DataRow("use key on door")]
    [DataRow("use the silver key on the oak door")]
    public void Door_UnlocksWithSilverKey(string input)
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.SilverKey);

        var response = game.Do(input);

        Assert.Contains("the oak door swings open", response.Text);
        Assert.IsFalse(game.State.IsLocked(LockIds.TreasuryDoor));
        game.Do("n");
        Assert.AreEqual(RoomIds.Treasury, game.Room);
    }

    [TestMethod]
    public void Door_UnlockWithoutKey()
    {
        var game = new TestGame().In(RoomIds.GreatHall);

        Assert.AreEqual("You have nothing that fits this lock.", game.Do("unlock door").Text);
        Assert.IsTrue(game.State.IsLocked(LockIds.TreasuryDoor));
    }

    [TestMethod]
    public void Door_OpenWithoutKey()
    {
        var game = new TestGame().In(RoomIds.GreatHall);

        Assert.AreEqual("It's locked.", game.Do("open door").Text);
    }

    [TestMethod]
    [DataRow("unlock door with gold key")]
    [DataRow("use gold key on door")]
    [DataRow("open door with gold key")]
    public void Door_WrongKey(string input)
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.GoldKey);

        Assert.AreEqual("The gold key is far too small for this lock.", game.Do(input).Text);
        Assert.IsTrue(game.State.IsLocked(LockIds.TreasuryDoor));
    }

    [TestMethod]
    public void Door_OnlyGoldKeyHeld_ImplicitUnlockSaysNothingFits()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.GoldKey);

        Assert.AreEqual("You have nothing that fits this lock.", game.Do("unlock door").Text);
    }

    [TestMethod]
    [DataRow("unlock door")]
    [DataRow("unlock door with key")]
    [DataRow("use key")]
    [DataRow("use key on door")]
    public void Door_BothKeysHeld_PicksSilverAutomatically(string input)
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.SilverKey, ItemIds.GoldKey);

        game.Do(input);

        Assert.IsFalse(game.State.IsLocked(LockIds.TreasuryDoor));
    }

    [TestMethod]
    public void Door_AlreadyUnlocked()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.SilverKey);
        game.Do("unlock door");

        Assert.AreEqual("It's already unlocked.", game.Do("unlock door").Text);
        Assert.AreEqual("It's already open.", game.Do("open door").Text);
    }

    [TestMethod]
    public void Door_KeyStaysInInventoryAfterUse()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.SilverKey);

        game.Do("unlock door");

        Assert.IsTrue(game.State.IsInInventory(ItemIds.SilverKey));
    }

    [TestMethod]
    public void Door_WithNonKeyItem()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.RustyPot);

        Assert.AreEqual("The rusty pot doesn't fit this lock.", game.Do("unlock door with pot").Text);
    }

    [TestMethod]
    public void Door_WithKeyNotCarried()
    {
        var game = new TestGame().In(RoomIds.GreatHall);

        Assert.AreEqual("You don't see any silver key here.", game.Do("unlock door with silver key").Text);
    }

    // ---- Treasure chest ----

    [TestMethod]
    [DataRow("open chest")]
    [DataRow("unlock chest")]
    [DataRow("open the treasure chest with the gold key")]
    [DataRow("unlock chest with key")]
    [DataRow("use gold key")]
    [DataRow("use key on chest")]
    public void Chest_GoldKeyWins(string input)
    {
        var game = new TestGame().In(RoomIds.Treasury).Holding(ItemIds.GoldKey);

        var response = game.Do(input);

        Assert.IsTrue(response.Won);
        Assert.IsTrue(game.State.HasWon);
        Assert.Contains("satisfying click", response.Text);
        Assert.Contains("golden light spills", response.Text);
    }

    [TestMethod]
    public void Chest_BothKeysHeld_PicksGoldAutomatically()
    {
        var game = new TestGame().In(RoomIds.Treasury).Holding(ItemIds.SilverKey, ItemIds.GoldKey);

        Assert.IsTrue(game.Do("open chest").Won);
    }

    [TestMethod]
    public void Chest_OpenWithoutKey()
    {
        var game = new TestGame().In(RoomIds.Treasury);

        var response = game.Do("open chest");

        Assert.AreEqual("It's locked.", response.Text);
        Assert.IsFalse(response.Won);
    }

    [TestMethod]
    [DataRow("unlock chest with silver key")]
    [DataRow("use silver key on chest")]
    public void Chest_SilverKeyDoesNotFit(string input)
    {
        var game = new TestGame().In(RoomIds.Treasury).Holding(ItemIds.SilverKey);

        var response = game.Do(input);

        Assert.AreEqual("The silver key doesn't fit this tiny golden lock.", response.Text);
        Assert.IsFalse(game.State.HasWon);
    }

    [TestMethod]
    public void AfterWinning_FurtherCommandsSayGameOver()
    {
        var game = new TestGame().In(RoomIds.Treasury).Holding(ItemIds.GoldKey);
        game.Do("open chest");

        var response = game.Do("s");

        Assert.Contains("adventure is over", response.Text);
        Assert.AreEqual(RoomIds.Treasury, game.Room);
    }

    // ---- Things without locks ----

    [TestMethod]
    public void Open_ThingWithoutLock()
    {
        Assert.AreEqual("You can't open that.", new TestGame().Do("open fountain").Text);
    }

    [TestMethod]
    public void Unlock_ThingWithoutLock()
    {
        Assert.AreEqual("You can't unlock that.", new TestGame().Do("unlock fountain").Text);
    }

    [TestMethod]
    public void UseKey_WhereNothingIsLocked()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey);

        Assert.AreEqual("There's nothing here to use that on.", game.Do("use silver key").Text);
    }

    [TestMethod]
    public void UseNonKey_NothingHappens()
    {
        var game = new TestGame().Holding(ItemIds.RustyPot);

        Assert.AreEqual("Nothing happens.", game.Do("use pot").Text);
    }
}
