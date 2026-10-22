using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>
/// ASK X ABOUT Y, or ASK ABOUT Y when one character is present. The topic phrase is matched
/// exactly against topic keywords; anything else gets the character's unknown-topic reply.
/// </summary>
public sealed class AskHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        var listener = DialogueActions.FindListener(context, command.DirectObject, out var failure);
        if (listener is null)
        {
            return GameResponse.Say(failure);
        }

        if (command.Preposition != "about" || command.IndirectObject is not { } subject)
        {
            return GameResponse.Say("Ask about what? Try ASK SKELETON ABOUT HAT.");
        }

        var topic = listener.Topics.FirstOrDefault(t => t.Keywords.Contains(subject));
        return topic is null
            ? GameResponse.Say(listener.UnknownTopicReply)
            : DialogueActions.Respond(context, listener, topic.Rules, listener.UnknownTopicReply);
    }
}
