using CastleAdventure.Content;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class ItemTests
{
    [TestMethod]
    public void Take_MovesItemToInventory()
    {
        var game = new TestGame().In(RoomIds.Kitchen);

        var response = game.Do("take the rusty pot");

        Assert.AreEqual("Taken.", response.Text);
        Assert.IsTrue(game.State.IsInInventory(ItemIds.RustyPot));
    }

    [TestMethod]
    [DataRow("get pot")]
    [DataRow("pick up pot")]
    [DataRow("TAKE POT")]
    [DataRow("grab the pot")]
    public void Take_Synonyms(string input)
    {
        var game = new TestGame().In(RoomIds.Kitchen);

        game.Do(input);

        Assert.IsTrue(game.State.IsInInventory(ItemIds.RustyPot));
    }

    [TestMethod]
    public void Take_AlreadyCarried()
    {
        var game = new TestGame().In(RoomIds.Kitchen);
        game.Do("take pot");

        Assert.AreEqual("You already have that.", game.Do("take pot").Text);
    }

    [TestMethod]
    [DataRow(RoomIds.Courtyard, "take fountain", "six strong friends")]
    [DataRow(RoomIds.Treasury, "take chest", "far too heavy")]
    [DataRow(RoomIds.Dungeon, "take hat", "Hands off the hat")]
    [DataRow(RoomIds.Dungeon, "take skeleton", "attached to this wall")]
    [DataRow(RoomIds.Dungeon, "take sir rattlebones", "attached to this wall")]
    public void Take_SceneryAndCharacters_RefuseInCharacter(string roomId, string input, string expected)
    {
        var game = new TestGame().In(roomId);

        Assert.Contains(expected, game.Do(input).Text);
    }

    [TestMethod]
    public void Take_HiddenSilverKey_IsNotHere()
    {
        var game = new TestGame().In(RoomIds.Chapel);

        Assert.AreEqual("You don't see any key here.", game.Do("take key").Text);
        Assert.IsFalse(game.State.IsInInventory(ItemIds.SilverKey));
    }

    [TestMethod]
    public void Take_RevealedSilverKey_Works()
    {
        var game = new TestGame().In(RoomIds.Chapel).Revealed(ItemIds.SilverKey);

        game.Do("take key");

        Assert.IsTrue(game.State.IsInInventory(ItemIds.SilverKey));
    }

    [TestMethod]
    public void Take_ItemInAnotherRoom_IsNotHere()
    {
        Assert.AreEqual("You don't see any pot here.", new TestGame().Do("take pot").Text);
    }

    [TestMethod]
    public void UnknownNoun_ReportsUnknownWord()
    {
        Assert.AreEqual("I don't know the word 'flux'.", new TestGame().Do("take flux capacitor").Text);
    }

    [TestMethod]
    public void Take_WithoutObject_AsksWhat()
    {
        Assert.AreEqual("Take what?", new TestGame().Do("take").Text);
    }

    [TestMethod]
    public void Drop_LeavesItemInRoom_AndItIsListed()
    {
        var game = new TestGame().Holding(ItemIds.RustyPot);

        var drop = game.Do("drop pot");
        var look = game.Do("look");

        Assert.AreEqual("Dropped.", drop.Text);
        Assert.AreEqual(ItemLocation.InRoom(RoomIds.Courtyard), game.State.ItemLocations[ItemIds.RustyPot]);
        Assert.Contains("You can see a rusty pot here.", look.Text);
    }

    [TestMethod]
    public void Drop_ThenTakeAgain()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey);

        game.DoAll("drop silver key", "n", "s", "take silver key");

        Assert.IsTrue(game.State.IsInInventory(ItemIds.SilverKey));
    }

    [TestMethod]
    public void Drop_ItemNotCarried()
    {
        var game = new TestGame().In(RoomIds.Kitchen);

        Assert.AreEqual("You aren't carrying that.", game.Do("drop pot").Text);
    }

    [TestMethod]
    public void Drop_ItemNowhereNear()
    {
        Assert.AreEqual("You don't see any pot here.", new TestGame().Do("drop pot").Text);
    }

    [TestMethod]
    public void Inventory_Empty()
    {
        var response = new TestGame().Do("i");

        Assert.AreEqual("You are empty-handed.", response.Text);
        Assert.IsFalse(response.CountedMove);
    }

    [TestMethod]
    public void Inventory_ListsCarriedItems()
    {
        var game = new TestGame().Holding(ItemIds.Diary, ItemIds.RustyPot);

        var text = game.Do("inventory").Text;

        Assert.Contains("You are carrying:", text);
        Assert.Contains("a dusty diary", text);
        Assert.Contains("a rusty pot", text);
    }

    [TestMethod]
    public void RoomListing_ShowsTakeableItemsButNotScenery()
    {
        var text = new TestGame().In(RoomIds.Library).Do("look").Text;

        Assert.Contains("You can see a dusty diary here.", text);
        Assert.DoesNotContain("bookshelves here", text);
    }

    [TestMethod]
    public void RoomListing_MultipleItems_JoinedWithAnd()
    {
        var game = new TestGame().Holding(ItemIds.RustyPot, ItemIds.Diary);
        game.DoAll("drop pot", "drop diary");

        Assert.Contains("You can see a rusty pot and a dusty diary here.", game.Do("look").Text);
    }

    [TestMethod]
    public void Examine_Item()
    {
        var text = new TestGame().In(RoomIds.Chapel).Do("x altar").Text;

        Assert.Contains("Something beneath the cloth catches the moonlight.", text);
    }

    [TestMethod]
    public void Examine_CarriedItem_FromAnyRoom()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey);

        Assert.Contains("crescent moon", game.Do("examine silver key").Text);
    }

    [TestMethod]
    public void Examine_Character_UsesStateDescription()
    {
        var text = new TestGame().In(RoomIds.Dungeon).Do("look at the skeleton").Text;

        Assert.Contains("You could try talking to it.", text);
    }

    [TestMethod]
    public void Examine_AmbiguousKey_AsksWhichOne()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey, ItemIds.GoldKey);

        Assert.AreEqual("Which key do you mean, the silver key or the gold key?", game.Do("examine key").Text);
        Assert.Contains("gold", game.Do("examine gold key").Text);
    }

    [TestMethod]
    public void Drop_AmbiguousKey_AsksWhichOne()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey, ItemIds.GoldKey);

        Assert.StartsWith("Which key do you mean", game.Do("drop key").Text);
    }

    [TestMethod]
    public void Read_Diary()
    {
        var game = new TestGame().Holding(ItemIds.Diary);

        Assert.Contains("where the faithful kneel", game.Do("read diary").Text);
    }

    [TestMethod]
    public void Read_ThingWithoutText()
    {
        Assert.AreEqual("There's nothing written on the fountain.", new TestGame().Do("read fountain").Text);
    }
}
