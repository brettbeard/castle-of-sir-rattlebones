using CastleAdventure.Content;
using CastleAdventure.Engine;
using CastleAdventure.Output;

// Console front end: title, intro, the read–execute–print loop, confirmations, and the victory screen.
// All game rules live in GameEngine; all console details live in ConsoleUi.

var ui = new ConsoleUi();

ConsoleUi.PrintRaw(GameText.Title);
ConsoleUi.BlankLine();
ui.Print(GameText.Byline);
ConsoleUi.BlankLine();
ui.Print(GameText.PressAnyKey);
ConsoleUi.WaitForKey();
ConsoleUi.BlankLine();
ui.Print(GameText.Intro);

while (PlayOneGame(ui))
{
    ConsoleUi.BlankLine();
}

ConsoleUi.BlankLine();
ui.Print(GameText.Farewell);

// Plays until the player wins, restarts, or quits. Returns true to start a new game.
static bool PlayOneGame(ConsoleUi ui)
{
    var engine = new GameEngine(CastleWorld.Build());
    ConsoleUi.BlankLine();
    ui.Print(engine.Start());

    while (true)
    {
        var input = ConsoleUi.Prompt();
        if (input is null)
        {
            return false; // End of input (e.g. a piped script).
        }

        var response = engine.Execute(input);

        if (response.RequestQuit)
        {
            if (ui.Confirm(GameText.ConfirmQuit, whenNoInput: true))
            {
                return false;
            }

            continue;
        }

        if (response.RequestRestart)
        {
            if (ui.Confirm(GameText.ConfirmRestart, whenNoInput: false))
            {
                return true;
            }

            continue;
        }

        ConsoleUi.BlankLine();
        ui.Print(response);

        if (response.Won)
        {
            ConsoleUi.BlankLine();
            ui.Print(GameText.Victory, TextStyle.Victory);
            ConsoleUi.BlankLine();
            ui.Print(GameText.Summary(engine.State.Moves, engine.State.HintsUsed));
            return ui.Confirm(GameText.PlayAgain, whenNoInput: false);
        }
    }
}
