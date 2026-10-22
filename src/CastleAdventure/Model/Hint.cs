namespace CastleAdventure.Model;

/// <summary>One HINT tier. The engine shows the first hint whose <paramref name="Applies"/> is true.</summary>
public sealed record Hint(Func<GameState, bool> Applies, string Text);
