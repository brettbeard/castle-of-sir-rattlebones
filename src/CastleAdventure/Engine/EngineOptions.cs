namespace CastleAdventure.Engine;

public sealed record EngineOptions
{
    /// <summary>The only source of randomness in the game. Pass a seeded instance for repeatable runs.</summary>
    public Random Random { get; init; } = Random.Shared;

    /// <summary>Random response variants and ambient lines. Off = always the canonical (first) line.</summary>
    public bool Flavor { get; init; } = true;

    /// <summary>Chance of an ambient line after a counted move is 1 in this number.</summary>
    public int AmbientOneIn { get; init; } = 6;
}
