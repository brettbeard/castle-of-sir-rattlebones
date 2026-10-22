using CastleAdventure.Content;
using CastleAdventure.Engine;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class FlavorTests
{
    private static TestGame Seeded(int seed, int ambientOneIn = 6) =>
        new(new EngineOptions { Random = new Random(seed), AmbientOneIn = ambientOneIn });

    [TestMethod]
    public void FlavorOff_NeverAddsAmbientLines()
    {
        var game = new TestGame();

        for (var i = 0; i < 50; i++)
        {
            var response = game.Do(i % 2 == 0 ? "n" : "s");

            Assert.StartsWith("Exits:", response.Blocks[^1].Text);
        }
    }

    [TestMethod]
    public void FlavorOn_EveryTurn_AddsAnAmbientLine()
    {
        var response = Seeded(1, ambientOneIn: 1).Do("look");

        Assert.Contains(response.Blocks[^1].Text, FlavorText.Create().Ambient);
    }

    [TestMethod]
    public void AmbientLines_NeverRepeatBackToBack()
    {
        var game = Seeded(3, ambientOneIn: 1);
        string? previous = null;

        for (var i = 0; i < 40; i++)
        {
            var line = game.Do("look").Blocks[^1].Text;
            Assert.AreNotEqual(previous, line, $"turn {i}");
            previous = line;
        }
    }

    [TestMethod]
    public void FlavorOn_AmbientNotAddedToUncountedCommands()
    {
        var response = Seeded(1, ambientOneIn: 1).Do("i");

        Assert.AreEqual("You are empty-handed.", response.Text);
    }

    [TestMethod]
    public void SameSeed_SameOutput()
    {
        string[] script = ["n", "w", "s", "e", "look", "xyzzy", "w"];

        var a = Seeded(7);
        var b = Seeded(7);

        foreach (var command in script)
        {
            Assert.AreEqual(a.Do(command).Text, b.Do(command).Text, command);
        }
    }

    [TestMethod]
    public void UnknownWordVariants_AllNameTheWord()
    {
        for (var seed = 0; seed < 20; seed++)
        {
            Assert.Contains("'frobnicate'", Seeded(seed).Do("frobnicate").Text);
        }
    }

    [TestMethod]
    public void NoExitVariants_AreFromThePool()
    {
        var pool = FlavorText.Create().NoExit;

        for (var seed = 0; seed < 20; seed++)
        {
            var text = Seeded(seed, ambientOneIn: 0).Do("west").Text;
            Assert.Contains(text, pool);
        }
    }

    [TestMethod]
    public void Bookshelves_ShowABookTitle()
    {
        var text = new TestGame().In(RoomIds.Library).Do("examine bookshelves").Text;

        Assert.Contains("One spine still reads:", text);
    }

    [TestMethod]
    public void FlavorOff_TalkAgain_IsCanonical()
    {
        var game = new TestGame().In(RoomIds.Dungeon);
        game.Do("talk");

        Assert.Contains("Lovely to see you again", game.Do("talk").Text);
    }
}
