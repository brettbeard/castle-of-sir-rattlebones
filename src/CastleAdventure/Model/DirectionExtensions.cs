namespace CastleAdventure.Model;

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };

    /// <summary>Lowercase name used in game text, e.g. "Exits: north, down."</summary>
    public static string DisplayName(this Direction direction) => direction switch
    {
        Direction.North => "north",
        Direction.South => "south",
        Direction.East => "east",
        Direction.West => "west",
        Direction.Up => "up",
        Direction.Down => "down",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };
}
