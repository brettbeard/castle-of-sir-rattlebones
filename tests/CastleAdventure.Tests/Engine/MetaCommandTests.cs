using CastleAdventure.Content;
using CastleAdventure.Parsing;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class MetaCommandTests
{
    [TestMethod]
    [DataRow("help")]
    [DataRow("?")]
    public void Help_ShowsReferenceCard(string input)
    {
        var response = new TestGame().Do(input);

        Assert.AreEqual(GameText.Help, response.Text);
        Assert.IsFalse(response.CountedMove);
    }

    [TestMethod]
    public void Restart_RequestsRestart_WithoutChangingState()
    {
        var game = new TestGame();
        game.Do("n");

        var response = game.Do("restart");

        Assert.IsTrue(response.RequestRestart);
        Assert.IsFalse(response.CountedMove);
        Assert.AreEqual(RoomIds.GreatHall, game.Room);
    }

    [TestMethod]
    public void NewGame_AfterRestart_IsFresh()
    {
        var old = new TestGame().Holding(ItemIds.SilverKey);
        old.Do("n");

        var fresh = new TestGame();

        Assert.AreEqual(RoomIds.Courtyard, fresh.Room);
        Assert.AreEqual(0, fresh.State.Moves);
        Assert.IsFalse(fresh.State.IsInInventory(ItemIds.SilverKey));
    }

    [TestMethod]
    public void EveryVerb_HasAHandler()
    {
        foreach (var phrase in Vocabulary.VerbPhrases.Keys)
        {
            var response = new TestGame().Do(phrase);

            Assert.IsTrue(response.Blocks.Count > 0 || response.RequestQuit || response.RequestRestart, phrase);
        }
    }

    [TestMethod]
    public void Summary_Pluralises()
    {
        Assert.AreEqual("You won in 14 moves using 0 hints.", GameText.Summary(14, 0));
        Assert.AreEqual("You won in 1 move using 1 hint.", GameText.Summary(1, 1));
    }
}
