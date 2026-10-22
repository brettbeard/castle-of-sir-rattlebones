namespace CastleAdventure.Content;

/// <summary>Fixed text for the title screen, intro, HELP, and victory. Shown by Program.cs (HELP via the engine).</summary>
public static class GameText
{
    public const string Title =
        """
                  |>>>                                  |>>>
                  |                                     |
              _  _|_  _                             _  _|_  _
             |;|_|;|_|;|                           |;|_|;|_|;|
             \\.    .  /                           \\.    .  /
              \\:  .  /_____________________________\\:  .  /
               ||:   |  []    []    []    []    []   ||:   |
               ||:.  |                               ||:.  |
               ||:  .|            ______             ||:  .|
               ||:   |           |      |            ||:   |
           ____||____|___________|______|____________||____|____

                       T H E   C A S T L E   O F
                    S I R   R A T T L E B O N E S
        """;

    public const string Byline = "A tiny adventure in the old style.";

    public const string PressAnyKey = "Press any key to begin...";

    public const string Intro =
        "Lost in a storm, you stumble through the night until lightning reveals the ruins of a castle on the hill. " +
        "Its gates hang open. Inside, they say, lies a treasure that no one has claimed in two hundred years, " +
        "and someone who has been waiting a very long time for a visitor.\n\n" +
        "Type HELP for a list of commands, or HINT if you get stuck.";

    public const string Help =
        "Type simple commands like:  GO NORTH, N, TAKE LAMP, EXAMINE ALTAR,\n" +
        "OPEN DOOR, UNLOCK CHEST WITH KEY, TALK TO SKELETON, ASK SKELETON ABOUT HAT\n" +
        "Other commands: LOOK (L), INVENTORY (I), READ, LIFT, HINT, RESTART, QUIT\n" +
        "Tip: EXAMINE everything. Castles keep their secrets close.";

    public const string Victory =
        "*** You have found the treasure of Castle Moonhollow! ***\n\n" +
        "Somewhere below, Sir Rattlebones lets out a delighted rattle of applause.";

    public const string ConfirmQuit = "Are you sure you want to quit? (Y/N)";

    public const string ConfirmRestart = "Restart from the beginning? (Y/N)";

    public const string PlayAgain = "Play again? (Y/N)";

    public const string Farewell = "Farewell, adventurer. The castle will wait for you.";

    public static string Summary(int moves, int hints) =>
        $"You won in {moves} {Plural(moves, "move")} using {hints} {Plural(hints, "hint")}.";

    private static string Plural(int count, string word) => count == 1 ? word : $"{word}s";
}
