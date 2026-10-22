using CastleAdventure.Content;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class SkeletonTests
{
    private static TestGame InDungeon() => new TestGame().In(RoomIds.Dungeon);

    private static string SkeletonState(TestGame game) => game.State.CharacterStates[CharacterIds.Skeleton];

    [TestMethod]
    [DataRow("talk to skeleton")]
    [DataRow("talk to the skeleton")]
    [DataRow("speak to sir rattlebones")]
    [DataRow("talk")]
    [DataRow("greet skeleton")]
    public void Talk_Introduces(string input)
    {
        var game = InDungeon();

        var response = game.Do(input);

        Assert.Contains("I'm Sir Rattlebones", response.Text);
        Assert.AreEqual(SkeletonStates.Introduced, SkeletonState(game));
    }

    [TestMethod]
    public void Talk_Again_IsFriendlyAndKeepsState()
    {
        var game = InDungeon();
        game.Do("talk to skeleton");

        var again = game.Do("talk to skeleton");

        Assert.Contains("Lovely to see you again", again.Text);
        Assert.AreEqual(SkeletonStates.Introduced, SkeletonState(game));
    }

    [TestMethod]
    public void AskAboutKey_BeforeTalking_Refuses()
    {
        var game = InDungeon();

        var response = game.Do("ask skeleton about key");

        Assert.Contains("We haven't even been introduced!", response.Text);
        Assert.IsFalse(game.State.IsInInventory(ItemIds.GoldKey));
        Assert.AreEqual(SkeletonStates.Unmet, SkeletonState(game));
    }

    [TestMethod]
    [DataRow("ask skeleton about key")]
    [DataRow("ask skeleton about the gold key")]
    [DataRow("ask sir rattlebones about the golden key")]
    [DataRow("ask about key")]
    [DataRow("ask skeleton for the key")]
    public void AskAboutKey_AfterTalking_GivesGoldKey(string input)
    {
        var game = InDungeon();
        game.Do("talk to skeleton");

        var response = game.Do(input);

        Assert.Contains("tips its hat", response.Text);
        Assert.IsTrue(game.State.IsInInventory(ItemIds.GoldKey));
        Assert.AreEqual(SkeletonStates.KeyGiven, SkeletonState(game));
        Assert.Contains("a gold key", game.Do("i").Text);
    }

    [TestMethod]
    public void AskAboutKey_AfterGiven_DoesNotGiveTwice()
    {
        var game = InDungeon();
        game.DoAll("talk to skeleton", "ask skeleton about key");

        var again = game.Do("ask skeleton about key");

        Assert.Contains("I gave you my only key", again.Text);
        Assert.AreEqual(SkeletonStates.KeyGiven, SkeletonState(game));
    }

    [TestMethod]
    [DataRow("take hat")]
    [DataRow("take key")]
    [DataRow("take gold key")]
    [DataRow("get the feathered hat")]
    public void TakingHatOrKey_BeforeItIsGiven_IsRefused(string input)
    {
        var game = InDungeon();

        var response = game.Do(input);

        Assert.Contains("Hands off the hat", response.Text);
        Assert.IsFalse(game.State.IsInInventory(ItemIds.GoldKey));
    }

    [TestMethod]
    [DataRow("hat", "phoenix feather")]
    [DataRow("castle", "Castle Moonhollow")]
    [DataRow("treasure", "In the chest in the treasury")]
    [DataRow("chest", "tiny golden key")]
    [DataRow("chains", "Rusted shut")]
    [DataRow("diary", "Wrote down everything")]
    [DataRow("coin", "Buy yourself something nice")]
    [DataRow("yourself", "Very retired")]
    [DataRow("silver key", "hid it in the chapel")]
    public void Topics_HaveReplies(string topic, string expected)
    {
        var game = InDungeon();
        game.Do("talk to skeleton");

        Assert.Contains(expected, game.Do($"ask skeleton about {topic}").Text);
    }

    [TestMethod]
    [DataRow("dragons")]
    [DataRow("the weather")]
    public void UnknownTopic_NeverHeardOfIt(string topic)
    {
        Assert.Contains("Never heard of it.", InDungeon().Do($"ask skeleton about {topic}").Text);
    }

    [TestMethod]
    public void Presence_ChangesAfterIntroduction()
    {
        var game = InDungeon();
        game.Do("talk to skeleton");

        var look = game.Do("look");

        Assert.Contains("clearly delighted to have company", look.Text);
        Assert.DoesNotContain("jaw creaks slowly open", look.Text);
    }

    [TestMethod]
    public void TalkingToNobody()
    {
        Assert.AreEqual("There's nobody here to talk to.", new TestGame().Do("talk").Text);
    }

    [TestMethod]
    public void TalkingToAThing()
    {
        Assert.AreEqual("The fountain isn't much of a conversationalist.", new TestGame().Do("talk to fountain").Text);
    }

    [TestMethod]
    public void AskWithoutTopic()
    {
        Assert.StartsWith("Ask about what?", InDungeon().Do("ask skeleton").Text);
    }

    [TestMethod]
    public void SkeletonInAnotherRoom_IsNotHere()
    {
        Assert.AreEqual("You don't see any skeleton here.", new TestGame().Do("talk to skeleton").Text);
    }
}
