using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>LOOK / L: always the full room description.</summary>
public sealed class LookHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        context.State.VisitedRoomIds.Add(context.State.CurrentRoomId);
        return GameResponse.Show(RoomDescriber.Describe(context, full: true));
    }
}
