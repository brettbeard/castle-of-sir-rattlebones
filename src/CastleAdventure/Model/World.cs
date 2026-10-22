namespace CastleAdventure.Model;

/// <summary>
/// The static definition of a game world, built once from content and never modified.
/// Everything that changes during play lives in <see cref="GameState"/>.
/// </summary>
public sealed class World
{
    /// <exception cref="InvalidOperationException">Two rooms, items, characters, or locks share an ID.</exception>
    public World(string startRoomId, IEnumerable<Room> rooms)
    {
        StartRoomId = startRoomId;

        var roomList = rooms.ToList();
        Rooms = ToUniqueDictionary(roomList, r => r.Id, "room");

        var characters = roomList.SelectMany(r => r.Characters).ToList();
        Characters = ToUniqueDictionary(characters, c => c.Id, "character");
        CharacterRoomIds = roomList
            .SelectMany(r => r.Characters.Select(c => (CharacterId: c.Id, RoomId: r.Id)))
            .ToDictionary(x => x.CharacterId, x => x.RoomId);

        var placedItems = roomList
            .SelectMany(r => r.Items.Select(i => (Item: i, Location: ItemLocation.InRoom(r.Id))))
            .Concat(characters.SelectMany(c => c.HeldItems.Select(i => (Item: i, Location: ItemLocation.HeldBy(c.Id)))))
            .ToList();
        Items = ToUniqueDictionary(placedItems.Select(p => p.Item), i => i.Id, "item");
        InitialItemLocations = placedItems.ToDictionary(p => p.Item.Id, p => p.Location);

        Locks = ToUniqueDictionary(Items.Values.Select(i => i.Lock).OfType<KeyLock>(), l => l.Id, "lock");
    }

    public string StartRoomId { get; }

    public IReadOnlyDictionary<string, Room> Rooms { get; }

    public IReadOnlyDictionary<string, Item> Items { get; }

    public IReadOnlyDictionary<string, Character> Characters { get; }

    public IReadOnlyDictionary<string, KeyLock> Locks { get; }

    /// <summary>Which room each character is in.</summary>
    public IReadOnlyDictionary<string, string> CharacterRoomIds { get; }

    /// <summary>Where each item starts: in a room or held by a character.</summary>
    public IReadOnlyDictionary<string, ItemLocation> InitialItemLocations { get; }

    private static Dictionary<string, T> ToUniqueDictionary<T>(IEnumerable<T> values, Func<T, string> getId, string kind)
    {
        var result = new Dictionary<string, T>();
        foreach (var value in values)
        {
            var id = getId(value);
            if (!result.TryAdd(id, value))
            {
                throw new InvalidOperationException($"Duplicate {kind} ID '{id}'.");
            }
        }

        return result;
    }
}
