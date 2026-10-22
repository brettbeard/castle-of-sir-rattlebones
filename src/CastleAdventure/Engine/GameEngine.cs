using CastleAdventure.Commands;
using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Engine;

/// <summary>
/// The game's single entry point: text in, <see cref="GameResponse"/> out. Never touches the console,
/// so tests and walkthrough scripts drive it directly. Also owns move counting and ambient lines.
/// </summary>
public sealed class GameEngine
{
    private readonly CommandDispatcher _dispatcher = new();
    private readonly GameContext _context;
    private readonly EngineOptions _options;

    public GameEngine(World world, EngineOptions? options = null)
    {
        _options = options ?? new EngineOptions();
        _context = new GameContext(
            world,
            GameState.NewGame(world),
            new NounResolver(world),
            new Flavor(_options.Random, _options.Flavor));
    }

    public World World => _context.World;

    public GameState State => _context.State;

    /// <summary>The opening room description. Call once before the first command.</summary>
    public GameResponse Start()
    {
        State.VisitedRoomIds.Add(State.CurrentRoomId);
        return GameResponse.Show(RoomDescriber.Describe(_context, full: true)) with { CountedMove = false };
    }

    public GameResponse Execute(string? input)
    {
        if (State.HasWon)
        {
            return GameResponse.Say("The treasure is yours. The adventure is over.") with { CountedMove = false, Won = true };
        }

        var command = Parser.Parse(input);
        var response = FirstUnknownNounWord(command) is { } word
            ? GameResponse.Say(_context.UnknownWordMessage(word)) with { CountedMove = false }
            : _dispatcher.Dispatch(command, _context);

        if (!response.CountedMove)
        {
            return response;
        }

        State.Moves++;
        return response.Won || !_context.Flavor.Roll(_options.AmbientOneIn)
            ? response
            : response with { Blocks = [.. response.Blocks, new TextBlock(_context.Flavor.Pick(World.Flavor.Ambient, string.Empty))] };
    }

    /// <summary>
    /// Words the game has never heard of get "I don't know the word…" and don't cost a move.
    /// ASK topics are exempt (the character answers in character) and so is "go to &lt;place&gt;".
    /// </summary>
    private string? FirstUnknownNounWord(ParsedCommand command)
    {
        if (command.Verb == Verb.Move)
        {
            return null;
        }

        string?[] phrases = command.Verb == Verb.Ask
            ? [command.DirectObject]
            : [command.DirectObject, command.IndirectObject];

        return phrases.OfType<string>().Select(_context.Nouns.FirstUnknownWord).FirstOrDefault(w => w is not null);
    }
}
