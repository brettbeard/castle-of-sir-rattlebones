using CastleAdventure.Content.Rooms;
using CastleAdventure.Model;

namespace CastleAdventure.Content;

/// <summary>Composes the castle from the per-room files. Each call returns a fresh world.</summary>
public static class CastleWorld
{
    public static World Build() => new(
        RoomIds.Courtyard,
        [
            Courtyard.Create(),
            GreatHall.Create(),
            Kitchen.Create(),
            Library.Create(),
            Chapel.Create(),
            Dungeon.Create(),
            Treasury.Create(),
        ]);
}
