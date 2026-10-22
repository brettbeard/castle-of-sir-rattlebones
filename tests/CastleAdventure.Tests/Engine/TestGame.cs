using CastleAdventure.Content;
using CastleAdventure.Engine;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Engine;

/// <summary>
/// A started castle game with shortcuts for setting up state in tests. Flavor is off by default,
/// so every response is the canonical line and no ambient lines appear.
/// </summary>
internal sealed class TestGame
{
    public TestGame(EngineOptions? options = null)
    {
        Engine = new GameEngine(CastleWorld.Build(), options ?? new EngineOptions { Flavor = false });
        Opening = Engine.Start();
    }

    public GameEngine Engine { get; }

    public GameResponse Opening { get; }

    public GameState State => Engine.State;

    public string Room => State.CurrentRoomId;

    public GameResponse Do(string input) => Engine.Execute(input);

    /// <summary>Runs several commands and returns the last response.</summary>
    public GameResponse DoAll(params string[] inputs)
    {
        GameResponse? last = null;
        foreach (var input in inputs)
        {
            last = Do(input);
        }

        return last ?? throw new ArgumentException("Give at least one command.", nameof(inputs));
    }

    public TestGame In(string roomId)
    {
        State.CurrentRoomId = roomId;
        State.VisitedRoomIds.Add(roomId);
        return this;
    }

    public TestGame Holding(params string[] itemIds)
    {
        foreach (var id in itemIds)
        {
            State.ItemLocations[id] = ItemLocation.Inventory;
            State.RevealedItemIds.Add(id);
        }

        return this;
    }

    public TestGame Revealed(string itemId)
    {
        State.RevealedItemIds.Add(itemId);
        return this;
    }

    public TestGame Unlocked(string lockId)
    {
        State.UnlockedLockIds.Add(lockId);
        return this;
    }
}
