using CastleAdventure.Model;

namespace CastleAdventure.Content.Characters;

/// <summary>
/// Sir Rattlebones: frightening at first, actually a chatty old knight.
/// Unmet → (TALK) → Introduced → (ASK ABOUT KEY) → KeyGiven.
/// </summary>
public static class Skeleton
{
    private const string Hat = "\"Hands off the hat, friend! A gentleman is nothing without his hat.\"";

    public static Character Create() => new()
    {
        Id = CharacterIds.Skeleton,
        Name = "skeleton",
        Aliases = { "sir rattlebones", "rattlebones", "sir", "bones", "knight" },
        States = { SkeletonStates.Unmet, SkeletonStates.Introduced, SkeletonStates.KeyGiven },
        InitialState = SkeletonStates.Unmet,
        PresenceByState =
        {
            [SkeletonStates.Unmet] =
                "Slumped against the far wall in rusted shackles is a skeleton wearing a battered feathered hat. " +
                "As you watch, its jaw creaks slowly open...",
            [SkeletonStates.Introduced] =
                "Sir Rattlebones lounges in its shackles, hat tilted jauntily, clearly delighted to have company.",
            [SkeletonStates.KeyGiven] =
                "Sir Rattlebones lounges in its shackles, humming an old drinking song slightly off-key.",
        },
        DescriptionByState =
        {
            [SkeletonStates.Unmet] =
                "Yellowed bones, empty eye sockets, and a grin that could mean anything. " +
                "A battered feathered hat sits on its skull. Its jaw is moving, very slightly. You could try talking to it.",
            [SkeletonStates.Introduced] =
                "Sir Rattlebones, knight of the realm (retired). Its grin seems friendlier now that you've been introduced.",
            [SkeletonStates.KeyGiven] =
                "Sir Rattlebones, knight of the realm (retired), looking very pleased with itself.",
        },
        Talk =
        {
            new DialogueRule(
                "The skeleton's head snaps toward you. You brace yourself...\n\n" +
                "\"Oh! A visitor! Forgive the rattling. I'm Sir Rattlebones, and I haven't had a proper chat " +
                "in two hundred years. Ask me anything! Well, anything about the castle. Or hats. I'm very good on hats.\"")
            {
                WhenStates = [SkeletonStates.Unmet],
                NewState = SkeletonStates.Introduced,
            },
            new DialogueRule(
                "\"Lovely to see you again! Ask me about anything: the castle, the treasure, my hat...\"")
            {
                Alternatives =
                [
                    "\"Ah, my favourite visitor! Also my only visitor. What shall we talk about?\"",
                    "\"Have I told you about my hat? Ask me about it. Go on.\"",
                    "\"Back again! The castle, the treasure, the steward, my hat. Pick a topic, any topic.\"",
                ],
            },
        },
        Topics =
        {
            new Topic
            {
                Id = TopicIds.Key,
                Keywords = { "key", "keys", "gold key", "golden key", "gold" },
                Rules =
                {
                    new DialogueRule("The skeleton's jaw clacks indignantly. \"We haven't even been introduced!\"")
                    {
                        WhenStates = [SkeletonStates.Unmet],
                    },
                    new DialogueRule(
                        "\"The key? Oh, THAT key.\" Sir Rattlebones tips its hat with a flourish, and a gold key tumbles " +
                        "out from under the crown. It catches the key neatly in one bony hand and presses it into yours. " +
                        "\"Been keeping it warm for whoever came along. Bones don't need gold.\"")
                    {
                        WhenStates = [SkeletonStates.Introduced],
                        NewState = SkeletonStates.KeyGiven,
                        GivesItemId = ItemIds.GoldKey,
                    },
                    new DialogueRule("\"I gave you my only key, friend. Check your pockets!\""),
                },
            },
            Simple(TopicIds.SilverKey, "\"The silver one? The old steward hid it in the chapel. Very devout. Terrible at hiding things.\"",
                "silver key", "silver"),
            Simple(TopicIds.Hat, "\"Splendid, isn't it? Genuine phoenix feather. Well, pheasant. Well, chicken.\"",
                "hat", "feathered hat", "feather", "plume"),
            Simple(TopicIds.Castle, "\"Castle Moonhollow! Finest castle in the valley, two hundred years ago. The roof was better then. So was I.\"",
                "castle", "moonhollow", "castle moonhollow"),
            Simple(TopicIds.Treasure, "\"Oh, there's treasure all right. In the chest in the treasury, north of the great hall. " +
                "The door wants a silver key and the chest a gold one. Very security-minded, the old lord.\"",
                "treasure", "treasury", "gold coins", "riches"),
            Simple(TopicIds.Chest, "\"Big iron thing in the treasury. Tiny golden lock. You'll want a tiny golden key, I'd imagine.\"",
                "chest", "treasure chest"),
            Simple(TopicIds.Chains, "\"These? Rusted shut decades ago. I've stopped minding. I haven't the legs for travel anyway. " +
                "Well, I have the legs. Just not the muscles.\"",
                "chains", "chain", "shackles"),
            Simple(TopicIds.Diary, "\"The steward's diary? Wrote down everything. Terrible habit for someone with secrets.\"",
                "diary", "journal", "steward"),
            Simple(TopicIds.Coin, "\"A copper coin! Keep it. Buy yourself something nice. Like a hat.\"",
                "coin", "copper coin", "copper"),
            Simple(TopicIds.Self, "\"Sir Rattlebones, knight of the realm, retired. Very retired. Extremely retired.\"",
                "yourself", "you", "self", "name", "sir rattlebones", "rattlebones", "skeleton", "knight"),
        },
        UnknownTopicReply = "The skeleton scratches its skull. \"Never heard of it.\"",
        TakeRefusal = "\"I'm flattered, but I'm quite attached to this wall.\"",
        HeldItems =
        {
            new Item
            {
                Id = ItemIds.GoldKey,
                Name = "gold key",
                Aliases = { "key", "gold", "golden key" },
                Description = "A small, heavy key of solid gold, still faintly warm from being under a hat.",
                IsTakeable = true,
                IsHidden = true,
            },
        },
        HeldItemTakeRefusal = Hat,
    };

    private static Topic Simple(string id, string reply, params string[] keywords)
    {
        var topic = new Topic { Id = id, Rules = { new DialogueRule(reply) } };
        topic.Keywords.AddRange(keywords);
        return topic;
    }
}
