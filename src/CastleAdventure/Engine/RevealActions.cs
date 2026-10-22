using CastleAdventure.Model;

namespace CastleAdventure.Engine;

/// <summary>Uncovers hidden items whose <see cref="RevealTrigger"/> matches what the player just did.</summary>
public static class RevealActions
{
    /// <summary>Reveals every still-hidden item triggered by acting on <paramref name="trigger"/>; returns their messages.</summary>
    public static IReadOnlyList<string> RevealFrom(GameContext context, Item trigger, RevealVerbs verb)
    {
        var messages = new List<string>();
        foreach (var item in context.World.Items.Values)
        {
            if (item.RevealedBy is { } reveal
                && reveal.TriggerItemId == trigger.Id
                && reveal.Verbs.HasFlag(verb)
                && !context.IsVisible(item))
            {
                context.State.RevealedItemIds.Add(item.Id);
                messages.Add(reveal.Message);
            }
        }

        return messages;
    }
}
