using CastleAdventure.Model;

namespace CastleAdventure.Tests.Model;

[TestClass]
public sealed class WorldTests
{
    [TestMethod]
    public void Constructor_IndexesRoomItemsAndCharacterHeldItems()
    {
        var character = TestWorldBuilder.Character("ghost");
        character.HeldItems.Add(TestWorldBuilder.Item("ring", takeable: true));

        var world = TestWorldBuilder.Minimal()
            .Configure("hall", r =>
            {
                r.Items.Add(TestWorldBuilder.Item("lamp", takeable: true));
                r.Characters.Add(character);
            })
            .Build();

        Assert.AreEqual(ItemLocation.InRoom("hall"), world.InitialItemLocations["lamp"]);
        Assert.AreEqual(ItemLocation.InRoom("vault"), world.InitialItemLocations["chest"]);
        Assert.AreEqual(ItemLocation.HeldBy("ghost"), world.InitialItemLocations["ring"]);
        Assert.AreEqual("hall", world.CharacterRoomIds["ghost"]);
    }

    [TestMethod]
    public void Constructor_CollectsLocksFromItems()
    {
        var world = TestWorldBuilder.Minimal()
            .Configure("hall", r => r.Items.Add(new Item
            {
                Id = "gate",
                Name = "gate",
                Description = "A gate.",
                Lock = new KeyLock { Id = "gate_lock", KeyId = "key", UnlockMessage = "Click." },
            }))
            .Build();

        Assert.IsTrue(world.Locks.ContainsKey("gate_lock"));
    }

    [TestMethod]
    public void Constructor_DuplicateRoomId_Throws()
    {
        var builder = new TestWorldBuilder().Room("hall").Room("hall");

        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());

        Assert.Contains("room ID 'hall'", ex.Message);
    }

    [TestMethod]
    public void Constructor_DuplicateItemIdAcrossRooms_Throws()
    {
        var builder = TestWorldBuilder.Minimal()
            .Configure("hall", r => r.Items.Add(TestWorldBuilder.Item("chest")));

        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());

        Assert.Contains("item ID 'chest'", ex.Message);
    }
}
