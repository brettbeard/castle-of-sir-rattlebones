namespace CastleAdventure.Engine;

/// <summary>One line or paragraph of output. May contain "\n\n" for paragraph breaks.</summary>
public sealed record TextBlock(string Text, TextStyle Style = TextStyle.Normal);
