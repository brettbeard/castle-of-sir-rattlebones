namespace CastleAdventure.Parsing;

public enum Verb
{
    /// <summary>Empty or whitespace-only input.</summary>
    None,

    /// <summary>First word isn't a known verb or direction; see <see cref="ParsedCommand.UnknownWord"/>.</summary>
    Unknown,

    Move,
    Look,
    Examine,
    Read,
    Take,
    Drop,
    Inventory,
    Open,
    Unlock,
    Use,

    /// <summary>LIFT / MOVE / SEARCH / LOOK UNDER — can reveal hidden items.</summary>
    Search,

    Talk,
    Ask,
    Help,
    Hint,
    Restart,
    Quit,
}
