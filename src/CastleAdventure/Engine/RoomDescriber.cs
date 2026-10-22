using CastleAdventure.Model;

namespace CastleAdventure.Engine;

/// <summary>
/// Builds the text for the current room: heading, prose (full mode only), characters,
/// takeable items lying here, and exits. Scenery is never listed; it belongs in the prose.
/// </summary>
public static class RoomDescriber
{
    public static IReadOnlyList<TextBlock> Describe(GameContext context, bool full)
    {
        var room = context.CurrentRoom;
        var blocks = new List<TextBlock> { new(room.Name, TextStyle.RoomName) };

        if (full)
        {
            blocks.Add(new TextBlock(room.Description));
        }

        foreach (var character in context.PresentCharacters)
        {
            blocks.Add(new TextBlock(character.PresenceByState[context.StateOf(character)]));
        }

        var items = context.VisibleItemsIn(room.Id).Where(i => i.IsTakeable).Select(i => i.ListName).ToList();
        if (items.Count > 0)
        {
            blocks.Add(new TextBlock($"You can see {Prose.JoinAnd(items)} here."));
        }

        blocks.Add(new TextBlock(DescribeExits(room)));
        return blocks;
    }

    private static string DescribeExits(Room room)
    {
        var exits = Enum.GetValues<Direction>().Where(room.Exits.ContainsKey).Select(d => d.DisplayName()).ToList();
        return exits.Count == 0 ? "There are no obvious exits." : $"Exits: {string.Join(", ", exits)}.";
    }
}
