namespace CastleAdventure.Engine;

/// <summary>Random choices for flavor text. When disabled, always returns the first option and never "rolls".</summary>
public sealed class Flavor(Random random, bool enabled)
{
    public bool Enabled { get; } = enabled;

    public string Pick(IReadOnlyList<string> options, string fallback) =>
        options.Count == 0 ? fallback
        : Enabled ? options[random.Next(options.Count)]
        : options[0];

    /// <summary>True with probability 1 in <paramref name="oneIn"/>, but only when flavor is enabled.</summary>
    public bool Roll(int oneIn) => Enabled && oneIn > 0 && random.Next(oneIn) == 0;
}
