using CastleAdventure.Content;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Content;

/// <summary>The real castle must pass every structural check. Fails with a readable list of problems.</summary>
[TestClass]
public sealed class WorldIntegrityTests
{
    [TestMethod]
    public void CastleWorld_Builds_WithUniqueIds()
    {
        // The World constructor throws on any duplicate ID.
        var world = CastleWorld.Build();

        Assert.IsNotNull(world);
    }

    [TestMethod]
    public void CastleWorld_PassesValidation()
    {
        var problems = WorldValidator.Validate(CastleWorld.Build());

        Assert.IsEmpty(problems, string.Join(Environment.NewLine, problems));
    }

    [TestMethod]
    public void CastleWorld_EveryRoomHasNameAndDescription()
    {
        foreach (var room in CastleWorld.Build().Rooms.Values)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(room.Name), room.Id);
            Assert.IsFalse(string.IsNullOrWhiteSpace(room.Description), room.Id);
        }
    }

    [TestMethod]
    public void CastleWorld_EveryItemHasDescription()
    {
        foreach (var item in CastleWorld.Build().Items.Values)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.Description), item.Id);
        }
    }

    [TestMethod]
    public void CastleWorld_EveryScenicItemHasCustomTakeRefusal()
    {
        var scenery = CastleWorld.Build().Items.Values.Where(i => !i.IsTakeable);

        foreach (var item in scenery)
        {
            Assert.IsNotNull(item.TakeRefusal, $"Scenery '{item.Id}' should have an in-character TakeRefusal.");
        }
    }

    [TestMethod]
    public void CastleWorld_BuildReturnsFreshInstances()
    {
        Assert.AreNotSame(CastleWorld.Build().Rooms[RoomIds.Courtyard], CastleWorld.Build().Rooms[RoomIds.Courtyard]);
    }
}
