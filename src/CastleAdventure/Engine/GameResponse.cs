namespace CastleAdventure.Engine;

/// <summary>What the engine returns for one command: text to show plus signals for <c>Program.cs</c>.</summary>
public sealed record GameResponse(IReadOnlyList<TextBlock> Blocks)
{
    /// <summary>Whether this command counts toward the move total (see design.md §9).</summary>
    public bool CountedMove { get; init; } = true;

    public bool RequestQuit { get; init; }

    public bool RequestRestart { get; init; }

    public bool Won { get; init; }

    /// <summary>All blocks joined with newlines, ignoring styles. Handy for tests and transcripts.</summary>
    public string Text => string.Join("\n", Blocks.Select(b => b.Text));

    public static GameResponse Say(params string[] lines) => new(lines.Select(l => new TextBlock(l)).ToList());

    public static GameResponse Show(IEnumerable<TextBlock> blocks) => new(blocks.ToList());
}
