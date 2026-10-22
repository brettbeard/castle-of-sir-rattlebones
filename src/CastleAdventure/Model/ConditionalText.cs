namespace CastleAdventure.Model;

/// <summary>
/// Text that replaces a default description once the game reaches some state,
/// e.g. the altar after the key under its cloth has been found.
/// </summary>
public sealed record ConditionalText(Func<GameState, bool> When, string Text);
