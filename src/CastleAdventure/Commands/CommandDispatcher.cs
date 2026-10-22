using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>Routes a parsed command to its handler. Register new verbs here.</summary>
public sealed class CommandDispatcher
{
    private readonly Dictionary<Verb, ICommandHandler> _handlers = new()
    {
        [Verb.Move] = new MoveHandler(),
        [Verb.Look] = new LookHandler(),
        [Verb.Examine] = new ExamineHandler(),
        [Verb.Read] = new ReadHandler(),
        [Verb.Take] = new TakeHandler(),
        [Verb.Drop] = new DropHandler(),
        [Verb.Inventory] = new InventoryHandler(),
        [Verb.Open] = new OpenHandler(),
        [Verb.Unlock] = new UnlockHandler(),
        [Verb.Use] = new UseHandler(),
        [Verb.Search] = new SearchHandler(),
        [Verb.Talk] = new TalkHandler(),
        [Verb.Ask] = new AskHandler(),
        [Verb.Help] = new HelpHandler(),
        [Verb.Hint] = new HintHandler(),
        [Verb.Restart] = new RestartHandler(),
        [Verb.Quit] = new QuitHandler(),
    };

    public GameResponse Dispatch(ParsedCommand command, GameContext context) => command.Verb switch
    {
        Verb.None => GameResponse.Say("Beg pardon?") with { CountedMove = false },
        Verb.Unknown => GameResponse.Say(context.UnknownWordMessage(command.UnknownWord ?? string.Empty)) with { CountedMove = false },
        _ => _handlers[command.Verb].Handle(command, context),
    };
}
