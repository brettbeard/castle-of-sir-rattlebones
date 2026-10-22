using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>INVENTORY / I. Does not count as a move.</summary>
public sealed class InventoryHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        var items = context.Inventory.Select(i => $"  {i.ListName}").ToList();
        var response = items.Count == 0
            ? GameResponse.Say("You are empty-handed.")
            : GameResponse.Say(["You are carrying:", .. items]);

        return response with { CountedMove = false };
    }
}
