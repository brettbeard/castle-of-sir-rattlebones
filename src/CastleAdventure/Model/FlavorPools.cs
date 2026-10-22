namespace CastleAdventure.Model;

/// <summary>
/// Random variants for common responses. The first entry of each pool is the canonical line,
/// used when flavor is turned off (as in most tests).
/// </summary>
public sealed class FlavorPools
{
    /// <summary>Format strings with <c>{0}</c> for the unknown word.</summary>
    public List<string> UnknownWord { get; } = [];

    /// <summary>When there is no exit in the chosen direction.</summary>
    public List<string> NoExit { get; } = [];

    /// <summary>Atmospheric lines occasionally added after a move.</summary>
    public List<string> Ambient { get; } = [];
}
