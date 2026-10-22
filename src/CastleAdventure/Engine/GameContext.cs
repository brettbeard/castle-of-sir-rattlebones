using CastleAdventure.Model;

namespace CastleAdventure.Engine;

/// <summary>
/// The world, the current play state, and common queries over them. Passed to every command handler.
/// </summary>
public sealed class GameContext(World world, GameState state, NounResolver nouns)
{
    public World World { get; } = world;

    public GameState State { get; } = state;

    public NounResolver Nouns { get; } = nouns;

    public Room CurrentRoom => World.Rooms[State.CurrentRoomId];

    /// <summary>Hidden items stay invisible until revealed.</summary>
    public bool IsVisible(Item item) => !item.IsHidden || State.RevealedItemIds.Contains(item.Id);

    public ItemLocation LocationOf(Item item) => State.ItemLocations[item.Id];

    /// <summary>Visible items currently in the given room, in world-definition order.</summary>
    public IEnumerable<Item> VisibleItemsIn(string roomId) =>
        World.Items.Values.Where(i => LocationOf(i) == ItemLocation.InRoom(roomId) && IsVisible(i));

    /// <summary>Items the player carries, in world-definition order.</summary>
    public IEnumerable<Item> Inventory =>
        World.Items.Values.Where(i => LocationOf(i).Kind == LocationKind.Inventory);

    public IEnumerable<Character> PresentCharacters => CurrentRoom.Characters;

    public string StateOf(Character character) => State.CharacterStates[character.Id];

    public void MoveItem(Item item, ItemLocation location) => State.ItemLocations[item.Id] = location;
}
