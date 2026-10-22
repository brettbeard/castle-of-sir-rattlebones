using CastleAdventure.Content;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class HintTests
{
    [TestMethod]
    public void Start_PointsToDiary()
    {
        Assert.Contains("diary in the library", new TestGame().Do("hint").Text);
    }

    [TestMethod]
    public void HoldingDiary_PointsToWhereTheFaithfulKneel()
    {
        Assert.Contains("faithful kneel", new TestGame().Holding(ItemIds.Diary).Do("hint").Text);
    }

    [TestMethod]
    public void HoldingSilverKey_PointsToDoor()
    {
        Assert.Contains("crescent-engraved door", new TestGame().Holding(ItemIds.SilverKey).Do("hint").Text);
    }

    [TestMethod]
    public void DoorOpen_SkeletonUnmet_PointsBelowKitchen()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey).Unlocked(LockIds.TreasuryDoor);

        Assert.Contains("beneath the kitchen", game.Do("hint").Text);
    }

    [TestMethod]
    public void DoorOpen_SkeletonIntroduced_SuggestsAskingAboutKey()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey).Unlocked(LockIds.TreasuryDoor);
        game.State.CharacterStates[CharacterIds.Skeleton] = SkeletonStates.Introduced;

        Assert.Contains("asked about a key", game.Do("hint").Text);
    }

    [TestMethod]
    public void HoldingBothKeys_DoorOpen_ToTheChest()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey, ItemIds.GoldKey).Unlocked(LockIds.TreasuryDoor);

        Assert.Contains("To the chest!", game.Do("hint").Text);
    }

    [TestMethod]
    public void GoldKeyGivenThenDropped_SaysSo()
    {
        var game = new TestGame().Holding(ItemIds.SilverKey).Unlocked(LockIds.TreasuryDoor);
        game.State.CharacterStates[CharacterIds.Skeleton] = SkeletonStates.KeyGiven;

        Assert.Contains("put it down somewhere", game.Do("hint").Text);
    }

    [TestMethod]
    public void Hints_AreCounted_ButAreNotMoves()
    {
        var game = new TestGame();

        var response = game.DoAll("hint", "hints");

        Assert.AreEqual(2, game.State.HintsUsed);
        Assert.AreEqual(0, game.State.Moves);
        Assert.IsFalse(response.CountedMove);
    }
}
