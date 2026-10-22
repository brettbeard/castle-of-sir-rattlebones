using CastleAdventure.Model;

namespace CastleAdventure.Parsing;

/// <summary>
/// The result of parsing one line of input. Noun phrases are lowercase, articles removed,
/// and joined with single spaces (e.g. "silver key"); the parser does not check they exist.
/// </summary>
/// <example>
/// "unlock the door with the silver key" → Verb=Unlock, DirectObject="door",
/// Preposition="with", IndirectObject="silver key".
/// </example>
public sealed record ParsedCommand(Verb Verb)
{
    /// <summary>Set for <see cref="Verb.Move"/> when a direction was given.</summary>
    public Direction? Direction { get; init; }

    public string? DirectObject { get; init; }

    /// <summary>Canonical preposition: with, on, to, about, at, or in.</summary>
    public string? Preposition { get; init; }

    public string? IndirectObject { get; init; }

    /// <summary>For <see cref="Verb.Unknown"/>: the word that wasn't recognised.</summary>
    public string? UnknownWord { get; init; }
}
