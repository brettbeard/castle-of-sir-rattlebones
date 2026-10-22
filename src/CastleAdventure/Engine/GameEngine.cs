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
    private string? _lastAmbient;

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
            : response with { Blocks = [.. response.Blocks, new TextBlock(NextAmbientLine())] };
    }

    /// <summary>A random ambient line, never the same one twice in a row.</summary>
    private string NextAmbientLine()
    {
        var choices = World.Flavor.Ambient.Where(l => l != _lastAmbient).ToList();
        _lastAmbient = _context.Flavor.Pick(choices, _lastAmbient ?? string.Empty);
        return _lastAmbient;
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

        var listener = command.Verb is Verb.Talk or Verb.Ask
            && command.DirectObject is { } d && Vocabulary.CharacterPronouns.Contains(d)
            ? null
            : command.DirectObject;

        string?[] phrases = command.Verb == Verb.Ask
            ? [listener]
            : [listener, command.IndirectObject];

        return phrases.OfType<string>().Select(_context.Nouns.FirstUnknownWord).FirstOrDefault(w => w is not null);
    }
}
