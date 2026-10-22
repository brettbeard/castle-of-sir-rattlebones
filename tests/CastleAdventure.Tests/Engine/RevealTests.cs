using CastleAdventure.Content;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class RevealTests
{
    [TestMethod]
    [DataRow("lift cloth")]
    [DataRow("move the cloth")]
    [DataRow("look under the altar cloth")]
    [DataRow("search cloth")]
    [DataRow("examine cloth")]
    [DataRow("x cloth")]
    public void AltarCloth_RevealsSilverKey(string input)
    {
        var game = new TestGame().In(RoomIds.Chapel);

        var response = game.Do(input);

        Assert.Contains("lies a silver key!", response.Text);
        Assert.Contains(ItemIds.SilverKey, game.State.RevealedItemIds);
    }

    [TestMethod]
    public void RevealedKey_IsListedAndTakeable()
    {
        var game = new TestGame().In(RoomIds.Chapel);

        game.Do("lift cloth");

        Assert.Contains("You can see a silver key here.", game.Do("look").Text);
        Assert.AreEqual("Taken.", game.Do("take key").Text);
    }

    [TestMethod]
    public void ExamineAltar_HintsButDoesNotReveal()
    {
        var game = new TestGame().In(RoomIds.Chapel);

        var response = game.Do("examine altar");

        Assert.Contains("Something beneath the cloth catches the moonlight.", response.Text);
        Assert.DoesNotContain(ItemIds.SilverKey, game.State.RevealedItemIds);
    }

    [TestMethod]
    public void RevealHappensOnlyOnce()
    {
        var game = new TestGame().In(RoomIds.Chapel);

        game.Do("lift cloth");
        var second = game.Do("lift cloth");

        Assert.AreEqual("You search the altar cloth but find nothing of interest.", second.Text);
    }

    [TestMethod]
    public void ExaminingFountain_RevealsCoin()
    {
        var game = new TestGame();

        var response = game.Do("examine fountain");

        Assert.Contains("copper coin", response.Text);
        Assert.AreEqual("Taken.", game.Do("take coin").Text);
    }

    [TestMethod]
    public void SearchingPlainScenery_FindsNothing()
    {
        var game = new TestGame().In(RoomIds.Kitchen);

        Assert.AreEqual("You search the trapdoor but find nothing of interest.", game.Do("search trapdoor").Text);
    }

    [TestMethod]
    public void SearchWithoutObject_AsksWhat()
    {
        Assert.AreEqual("Search what?", new TestGame().Do("lift").Text);
    }
}
