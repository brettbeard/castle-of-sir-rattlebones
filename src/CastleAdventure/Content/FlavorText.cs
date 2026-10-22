using CastleAdventure.Model;

namespace CastleAdventure.Content;

/// <summary>Random response variants. The first line of each pool is the canonical one (used when flavor is off).</summary>
public static class FlavorText
{
    public static FlavorPools Create() => new()
    {
        UnknownWord =
        {
            "I don't know the word '{0}'.",
            "The castle echoes '{0}' back at you, baffled.",
            "'{0}'? That word means nothing within these walls.",
        },
        NoExit =
        {
            "You can't go that way.",
            "A solid wall disagrees.",
            "There's no way through in that direction.",
        },
        Ambient =
        {
            "Somewhere, a chain rattles.",
            "Wind moans through the arrow slits.",
            "A bat flits past overhead.",
            "A distant door bangs in the wind.",
            "The castle settles with a long, creaking sigh.",
        },
    };
}
