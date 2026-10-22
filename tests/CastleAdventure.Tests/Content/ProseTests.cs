using CastleAdventure.Content;
using CastleAdventure.Model;
using CastleAdventure.Tests.Engine;

namespace CastleAdventure.Tests.Content;

/// <summary>Nouns mentioned in room prose can be examined, and text keeps up with the player's progress.</summary>
[TestClass]
public sealed class ProseTests
{
    [TestMethod]
    [DataRow(RoomIds.Courtyard, "weeds")]
    [DataRow(RoomIds.Courtyard, "fountain")]
    [DataRow(RoomIds.Courtyard, "stone fish")]
    [DataRow(RoomIds.Courtyard, "moon")]
    [DataRow(RoomIds.Courtyard, "great doors")]
    [DataRow(RoomIds.GreatHall, "banquet table")]
    [DataRow(RoomIds.GreatHall, "banners")]
    [DataRow(RoomIds.GreatHall, "rafters")]
    [DataRow(RoomIds.GreatHall, "archways")]
    [DataRow(RoomIds.GreatHall, "oak door")]
    [DataRow(RoomIds.Kitchen, "hearth")]
    [DataRow(RoomIds.Kitchen, "ashes")]
    [DataRow(RoomIds.Kitchen, "trapdoor")]
    [DataRow(RoomIds.Kitchen, "steps")]
    [DataRow(RoomIds.Library, "bookshelves")]
    [DataRow(RoomIds.Library, "spines")]
    [DataRow(RoomIds.Library, "reading desk")]
    [DataRow(RoomIds.Library, "window")]
    [DataRow(RoomIds.Chapel, "stained glass window")]
    [DataRow(RoomIds.Chapel, "flagstones")]
    [DataRow(RoomIds.Chapel, "altar")]
    [DataRow(RoomIds.Chapel, "cloth")]
    [DataRow(RoomIds.Dungeon, "straw")]
    [DataRow(RoomIds.Dungeon, "water")]
    [DataRow(RoomIds.Dungeon, "chains")]
    [DataRow(RoomIds.Dungeon, "steps")]
    [DataRow(RoomIds.Dungeon, "skeleton")]
    [DataRow(RoomIds.Dungeon, "hat")]
    [DataRow(RoomIds.Treasury, "empty shelves")]
    [DataRow(RoomIds.Treasury, "chest")]
    [DataRow(RoomIds.Treasury, "suit of armour")]
    public void ProseNoun_CanBeExamined(string roomId, string noun)
    {
        var text = new TestGame().In(roomId).Do($"examine {noun}").Text;

        Assert.DoesNotStartWith("You don't see", text);
        Assert.DoesNotStartWith("I don't know", text);
    }

    [TestMethod]
    public void Altar_AfterKeyFound_NoLongerHintsAtSomethingBeneath()
    {
        var game = new TestGame().In(RoomIds.Chapel);
        game.Do("lift cloth");

        var text = game.Do("examine altar").Text;

        Assert.DoesNotContain("Something beneath the cloth", text);
        Assert.Contains("cloth pushed aside", text);
    }

    [TestMethod]
    public void Hat_AfterKeyGiven_NoLongerBulges()
    {
        var game = new TestGame().In(RoomIds.Dungeon);
        game.DoAll("talk to skeleton", "ask skeleton about key");

        var text = game.Do("examine hat").Text;

        Assert.DoesNotContain("bulges", text);
    }

    [TestMethod]
    public void GreatHall_AndDoor_ReflectUnlockedDoor()
    {
        var game = new TestGame().In(RoomIds.GreatHall).Holding(ItemIds.SilverKey);
        Assert.Contains("stands firmly shut", game.Do("look").Text);

        game.Do("unlock door");

        Assert.Contains("the heavy oak door stands open", game.Do("look").Text);
        Assert.Contains("It stands open", game.Do("examine door").Text);
    }

    [TestMethod]
    public void Fountain_AfterCoinFound_SaysNothingElseGlints()
    {
        var game = new TestGame();
        game.Do("examine fountain");

        Assert.Contains("Nothing else glints", game.Do("examine fountain").Text);
    }

    [TestMethod]
    [DataRow(RoomIds.Kitchen, "open trapdoor", "already open")]
    [DataRow(RoomIds.Courtyard, "open doors", "already hanging open")]
    [DataRow(RoomIds.Library, "open diary", "where the faithful kneel")]
    public void Open_OnThingsThatAreAlreadyOpenOrReadable(string roomId, string input, string expected)
    {
        Assert.Contains(expected, new TestGame().In(roomId).Do(input).Text);
    }

    [TestMethod]
    public void EveryRoomDescription_MentionsEveryVisibleExitDirection()
    {
        var world = CastleWorld.Build();
        foreach (var room in world.Rooms.Values)
        {
            foreach (var direction in room.Exits.Keys)
            {
                var word = direction.DisplayName();
                var mentioned = room.Description.Contains(word, StringComparison.OrdinalIgnoreCase)
                    || (direction == Direction.Down && room.Description.Contains("down", StringComparison.OrdinalIgnoreCase))
                    || (direction == Direction.Up && room.Description.Contains("up", StringComparison.OrdinalIgnoreCase));

                Assert.IsTrue(mentioned, $"{room.Id} prose doesn't mention its {word} exit.");
            }
        }
    }
}
