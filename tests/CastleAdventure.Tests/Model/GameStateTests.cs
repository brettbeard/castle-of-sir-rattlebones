using CastleAdventure.Content;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Model;

[TestClass]
public sealed class GameStateTests
{
    [TestMethod]
    public void NewGame_StartsInStartRoomWithNothing()
    {
        var state = GameState.NewGame(CastleWorld.Build());

        Assert.AreEqual(RoomIds.Courtyard, state.CurrentRoomId);
        Assert.IsFalse(state.ItemLocations.Values.Any(l => l.Kind == LocationKind.Inventory));
        Assert.IsEmpty(state.RevealedItemIds);
        Assert.IsEmpty(state.VisitedRoomIds);
        Assert.AreEqual(0, state.Moves);
        Assert.AreEqual(0, state.HintsUsed);
        Assert.IsFalse(state.HasWon);
    }

    [TestMethod]
    public void NewGame_AllLocksStartLocked()
    {
        var world = CastleWorld.Build();
        var state = GameState.NewGame(world);

        foreach (var lockId in world.Locks.Keys)
        {
            Assert.IsTrue(state.IsLocked(lockId), lockId);
        }
    }

    [TestMethod]
    public void NewGame_CharactersStartInInitialState()
    {
        var state = GameState.NewGame(CastleWorld.Build());

        Assert.AreEqual(SkeletonStates.Unmet, state.CharacterStates[CharacterIds.Skeleton]);
    }

    [TestMethod]
    public void NewGame_CopiesItemLocations_SoPlayDoesNotChangeTheWorld()
    {
        var world = CastleWorld.Build();
        var state = GameState.NewGame(world);

        state.ItemLocations[ItemIds.RustyPot] = ItemLocation.Inventory;

        Assert.IsTrue(state.IsInInventory(ItemIds.RustyPot));
        Assert.AreEqual(ItemLocation.InRoom(RoomIds.Kitchen), world.InitialItemLocations[ItemIds.RustyPot]);
        Assert.IsFalse(GameState.NewGame(world).IsInInventory(ItemIds.RustyPot));
    }
}
