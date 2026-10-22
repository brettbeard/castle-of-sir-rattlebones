using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>NORTH, GO WEST, D... Locked exits show their blocked message. Revisits get the brief description.</summary>
public sealed class MoveHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        if (command.Direction is not { } direction)
        {
            return GameResponse.Say("Which way? Try NORTH, SOUTH, EAST, WEST, UP or DOWN.");
        }

        if (!context.CurrentRoom.Exits.TryGetValue(direction, out var exit))
        {
            return GameResponse.Say(context.Flavor.Pick(context.World.Flavor.NoExit, "You can't go that way."));
        }

        if (exit.LockId is { } lockId && context.State.IsLocked(lockId))
        {
            return GameResponse.Say(exit.BlockedMessage ?? "The way is locked.");
        }

        context.State.CurrentRoomId = exit.TargetRoomId;
        var firstVisit = context.State.VisitedRoomIds.Add(exit.TargetRoomId);
        return GameResponse.Show(RoomDescriber.Describe(context, full: firstVisit));
    }
}
