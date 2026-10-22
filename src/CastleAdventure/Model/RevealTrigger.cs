namespace CastleAdventure.Model;

/// <summary>
/// Reveals a hidden item when the player acts on another item, e.g. lifting the altar cloth
/// reveals the silver key.
/// </summary>
/// <param name="TriggerItemId">The item the player must act on.</param>
/// <param name="Verbs">Which actions on the trigger item cause the reveal.</param>
/// <param name="Message">Shown (after the trigger item's own text) when the reveal happens.</param>
public sealed record RevealTrigger(string TriggerItemId, RevealVerbs Verbs, string Message);
