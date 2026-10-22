using CastleAdventure.Model;

namespace CastleAdventure.Tests.Model;

[TestClass]
public sealed class DirectionTests
{
    [TestMethod]
    [DataRow(Direction.North, Direction.South)]
    [DataRow(Direction.South, Direction.North)]
    [DataRow(Direction.East, Direction.West)]
    [DataRow(Direction.West, Direction.East)]
    [DataRow(Direction.Up, Direction.Down)]
    [DataRow(Direction.Down, Direction.Up)]
    public void Opposite_ReturnsReverseDirection(Direction direction, Direction expected)
    {
        Assert.AreEqual(expected, direction.Opposite());
    }

    [TestMethod]
    [DataRow(Direction.North, "north")]
    [DataRow(Direction.South, "south")]
    [DataRow(Direction.East, "east")]
    [DataRow(Direction.West, "west")]
    [DataRow(Direction.Up, "up")]
    [DataRow(Direction.Down, "down")]
    public void DisplayName_IsLowercaseWord(Direction direction, string expected)
    {
        Assert.AreEqual(expected, direction.DisplayName());
    }
}
