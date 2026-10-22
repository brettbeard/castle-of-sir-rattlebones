using CastleAdventure.Content;

namespace CastleAdventure.Tests.Engine;

/// <summary>design.md §9: known-word commands count even when they fail; typos and meta-commands don't.</summary>
[TestClass]
public sealed class MoveCounterTests
{
    [TestMethod]
    [DataRow("n")]
    [DataRow("w")]
    [DataRow("look")]
    [DataRow("examine fountain")]
    [DataRow("take fountain")]
    [DataRow("take")]
    [DataRow("go")]
    [DataRow("go to the kitchen")]
    [DataRow("talk")]
    [DataRow("take pot")]
    public void Counted(string input)
    {
        var game = new TestGame();

        game.Do(input);

        Assert.AreEqual(1, game.State.Moves, input);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("xyzzy")]
    [DataRow("take flux capacitor")]
    [DataRow("examine the gizmo")]
    [DataRow("help")]
    [DataRow("?")]
    [DataRow("hint")]
    [DataRow("i")]
    [DataRow("inventory")]
    [DataRow("restart")]
    [DataRow("quit")]
    public void NotCounted(string input)
    {
        var game = new TestGame();

        game.Do(input);

        Assert.AreEqual(0, game.State.Moves, input);
    }

    [TestMethod]
    public void UnknownNounWord_GetsUnknownWordReply()
    {
        Assert.AreEqual("I don't know the word 'gizmo'.", new TestGame().Do("examine the gizmo").Text);
    }

    [TestMethod]
    public void AskAboutUnknownTopic_IsAnsweredInCharacter_AndCounts()
    {
        var game = new TestGame().In(RoomIds.Dungeon);

        var response = game.Do("ask skeleton about dragons");

        Assert.Contains("Never heard of it.", response.Text);
        Assert.AreEqual(1, game.State.Moves);
    }

    [TestMethod]
    public void WinningMove_IsCounted()
    {
        var game = new TestGame().In(RoomIds.Treasury).Holding(ItemIds.GoldKey);

        game.Do("open chest");

        Assert.AreEqual(1, game.State.Moves);
    }
}
