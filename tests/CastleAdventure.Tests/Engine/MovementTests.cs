using CastleAdventure.Content;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class MovementTests
{
    [TestMethod]
    public void Start_ShowsCourtyardInFull()
    {
        var game = new TestGame();

        Assert.AreEqual(RoomIds.Courtyard, game.Room);
        Assert.StartsWith("Courtyard", game.Opening.Text);
        Assert.Contains("overgrown courtyard", game.Opening.Text);
        Assert.Contains("Exits: north, east.", game.Opening.Text);
    }

    [TestMethod]
    public void EveryExit_WorksWithFullDirectionNames()
    {
        AssertEveryExitWorks(d => d.DisplayName());
    }

    [TestMethod]
    public void EveryExit_WorksWithAbbreviations()
    {
        AssertEveryExitWorks(d => d.DisplayName()[..1]);
    }

    [TestMethod]
    public void EveryExit_WorksWithGo()
    {
        AssertEveryExitWorks(d => $"go {d.DisplayName()}");
    }

    [TestMethod]
    public void KitchenDown_ReachesDungeon_AndUpReturns()
    {
        var game = new TestGame().In(RoomIds.Kitchen);

        game.Do("down");
        Assert.AreEqual(RoomIds.Dungeon, game.Room);

        game.Do("u");
        Assert.AreEqual(RoomIds.Kitchen, game.Room);
    }

    [TestMethod]
    public void NoExit_StaysPut()
    {
        var game = new TestGame();

        var response = game.Do("west");

        Assert.AreEqual(RoomIds.Courtyard, game.Room);
        Assert.AreEqual("You can't go that way.", response.Text);
    }

    [TestMethod]
    public void TreasuryDoor_LockedBlocksNorth()
    {
        var game = new TestGame().In(RoomIds.GreatHall);

        var response = game.Do("n");

        Assert.AreEqual(RoomIds.GreatHall, game.Room);
        Assert.Contains("heavy oak door bars the way north", response.Text);
        Assert.Contains("silver crescent", response.Text);
    }

    [TestMethod]
    public void TreasuryDoor_UnlockedAllowsNorth()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Unlocked(LockIds.TreasuryDoor);

        game.Do("n");

        Assert.AreEqual(RoomIds.Treasury, game.Room);
    }

    [TestMethod]
    public void GoWithoutDirection_AsksWhichWay()
    {
        Assert.StartsWith("Which way?", new TestGame().Do("go").Text);
    }

    [TestMethod]
    public void FirstVisit_IsFull_RevisitIsBrief()
    {
        var game = new TestGame();

        var first = game.Do("n");
        game.Do("s");
        var second = game.Do("n");

        Assert.Contains("banquet table runs down its length", first.Text);
        Assert.DoesNotContain("banquet table runs down its length", second.Text);
        Assert.StartsWith("Great Hall", second.Text);
        Assert.Contains("Exits: north, south, east, west.", second.Text);
    }

    [TestMethod]
    public void Look_AlwaysShowsFullDescription()
    {
        var game = new TestGame();
        game.DoAll("n", "s");

        var look = game.Do("look");

        Assert.Contains("overgrown courtyard", look.Text);
    }

    [TestMethod]
    public void RoomName_IsStyledAsRoomName()
    {
        var response = new TestGame().Do("n");

        Assert.AreEqual(global::CastleAdventure.Engine.TextStyle.RoomName, response.Blocks[0].Style);
        Assert.AreEqual("Great Hall", response.Blocks[0].Text);
    }

    [TestMethod]
    public void Dungeon_ShowsSkeletonPresence()
    {
        var response = new TestGame().In(RoomIds.Kitchen).Do("d");

        Assert.Contains("skeleton wearing a battered feathered hat", response.Text);
        Assert.Contains("Exits: up.", response.Text);
    }

    private static void AssertEveryExitWorks(Func<Direction, string> command)
    {
        var world = CastleWorld.Build();
        foreach (var room in world.Rooms.Values)
        {
            foreach (var (direction, exit) in room.Exits)
            {
                var game = new TestGame().In(room.Id).Unlocked(LockIds.TreasuryDoor);

                game.Do(command(direction));

                Assert.AreEqual(exit.TargetRoomId, game.Room, $"{room.Id} --{command(direction)}--> ?");
            }
        }
    }
}
