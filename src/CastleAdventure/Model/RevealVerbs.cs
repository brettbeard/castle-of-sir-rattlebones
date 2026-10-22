namespace CastleAdventure.Model;

/// <summary>Which player actions on a trigger item reveal a hidden item.</summary>
[Flags]
public enum RevealVerbs
{
    None = 0,

    /// <summary>EXAMINE / X / LOOK AT.</summary>
    Examine = 1,

    /// <summary>LIFT / MOVE / SEARCH.</summary>
    Search = 2,
}
