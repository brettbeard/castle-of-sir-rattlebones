using CastleAdventure.Commands;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Engine;

/// <summary>
/// The game's single entry point: text in, <see cref="GameResponse"/> out. Never touches the console,
/// so tests and walkthrough scripts drive it directly.
/// </summary>
public sealed class GameEngine
{
    private readonly CommandDispatcher _dispatcher = new();
    private readonly GameContext _context;

    public GameEngine(World world)
    {
        _context = new GameContext(world, GameState.NewGame(world), new NounResolver(world));
    }

    public World World => _context.World;

    public GameState State => _context.State;

    /// <summary>The opening room description. Call once before the first command.</summary>
    public GameResponse Start()
    {
        State.VisitedRoomIds.Add(State.CurrentRoomId);
        return GameResponse.Show(RoomDescriber.Describe(_context, full: true)) with { CountedMove = false };
    }

    public GameResponse Execute(string? input) => State.HasWon
        ? GameResponse.Say("The treasure is yours. The adventure is over.") with { CountedMove = false, Won = true }
        : _dispatcher.Dispatch(Parser.Parse(input), _context);
}
