using CastleAdventure.Model;

namespace CastleAdventure.Tests;

/// <summary>
/// Builds small custom worlds for tests. Rooms get placeholder names and descriptions;
/// the first room added is the start room unless <see cref="StartIn"/> is called.
/// </summary>
internal sealed class TestWorldBuilder
{
    private readonly List<Room> _rooms = [];
    private string? _startRoomId;

    /// <summary>Two connected rooms (hall north to vault) and a win-trigger chest in the vault. Passes validation.</summary>
    public static TestWorldBuilder Minimal() => new TestWorldBuilder()
        .Room("hall")
        .Room("vault", r => r.Items.Add(WinTrigger()))
        .Connect("hall", Direction.North, "vault");

    public TestWorldBuilder Room(string id, Action<Room>? configure = null)
    {
        var room = new Room { Id = id, Name = id, Description = $"The {id}." };
        configure?.Invoke(room);
        _rooms.Add(room);
        _startRoomId ??= id;
        return this;
    }

    /// <summary>Adds an exit and its matching return exit.</summary>
    public TestWorldBuilder Connect(string fromId, Direction direction, string toId)
    {
        GetRoom(fromId).Exits[direction] = new Exit(toId);
        GetRoom(toId).Exits[direction.Opposite()] = new Exit(fromId);
        return this;
    }

    /// <summary>Adds an exit with no return exit (for testing validation).</summary>
    public TestWorldBuilder OneWay(string fromId, Direction direction, string toId)
    {
        GetRoom(fromId).Exits[direction] = new Exit(toId);
        return this;
    }

    public TestWorldBuilder Configure(string roomId, Action<Room> configure)
    {
        configure(GetRoom(roomId));
        return this;
    }

    public TestWorldBuilder StartIn(string roomId)
    {
        _startRoomId = roomId;
        return this;
    }

    public World Build() => new(_startRoomId ?? throw new InvalidOperationException("Add a room first."), _rooms);

    public static Item Item(string id, bool takeable = false) => new()
    {
        Id = id,
        Name = id.Replace('_', ' '),
        Description = $"A {id}.",
        IsTakeable = takeable,
    };

    public static Item WinTrigger(string id = "chest") => new()
    {
        Id = id,
        Name = id,
        Description = $"A {id}.",
        IsWinTrigger = true,
    };

    /// <summary>A character with one state ("idle") and all required text filled in.</summary>
    public static Character Character(string id) => new()
    {
        Id = id,
        Name = id,
        States = { "idle" },
        InitialState = "idle",
        PresenceByState = { ["idle"] = $"A {id} is here." },
        DescriptionByState = { ["idle"] = $"A {id}." },
        UnknownTopicReply = "Hmm?",
        TakeRefusal = "No.",
    };

    private Room GetRoom(string id) =>
        _rooms.SingleOrDefault(r => r.Id == id) ?? throw new InvalidOperationException($"No room '{id}' added yet.");
}
