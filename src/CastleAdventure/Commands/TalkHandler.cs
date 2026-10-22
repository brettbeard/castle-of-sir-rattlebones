using CastleAdventure.Engine;
using CastleAdventure.Parsing;

namespace CastleAdventure.Commands;

/// <summary>TALK TO X (or just TALK when one character is present). May change the character's state.</summary>
public sealed class TalkHandler : ICommandHandler
{
    public GameResponse Handle(ParsedCommand command, GameContext context)
    {
        var listener = DialogueActions.FindListener(context, command.DirectObject, out var failure);
        return listener is null
            ? GameResponse.Say(failure)
            : DialogueActions.Respond(context, listener, listener.Talk, "It has nothing to say.");
    }
}
