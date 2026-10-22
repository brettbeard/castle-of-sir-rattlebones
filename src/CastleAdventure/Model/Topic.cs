namespace CastleAdventure.Model;

/// <summary>Something the player can ASK a character ABOUT.</summary>
public sealed class Topic
{
    public required string Id { get; init; }

    /// <summary>Lowercase phrases that select this topic; matched exactly against the player's topic.</summary>
    public List<string> Keywords { get; } = [];

    /// <summary>Replies in priority order; see <see cref="DialogueRule"/>.</summary>
    public List<DialogueRule> Rules { get; } = [];
}
