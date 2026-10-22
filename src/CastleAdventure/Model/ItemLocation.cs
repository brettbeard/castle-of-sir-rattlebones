namespace CastleAdventure.Model;

/// <summary>Where an item currently is. <see cref="OwnerId"/> is the room or character ID.</summary>
public readonly record struct ItemLocation(LocationKind Kind, string? OwnerId)
{
    public static ItemLocation Nowhere { get; } = new(LocationKind.Nowhere, null);

    public static ItemLocation Inventory { get; } = new(LocationKind.Inventory, null);

    public static ItemLocation InRoom(string roomId) => new(LocationKind.Room, roomId);

    public static ItemLocation HeldBy(string characterId) => new(LocationKind.Character, characterId);
}
