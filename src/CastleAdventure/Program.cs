using CastleAdventure.Content;
using CastleAdventure.Engine;

// Console front end. Phase 4: plain read–execute–print loop.
// Title screen, colours, wrapping, and Y/N confirmations arrive in Phase 6.

var engine = new GameEngine(CastleWorld.Build());

Console.WriteLine("THE CASTLE OF SIR RATTLEBONES");
Console.WriteLine();
Print(engine.Start());

while (true)
{
    Console.WriteLine();
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input is null)
    {
        break; // End of input (e.g. piped script).
    }

    var response = engine.Execute(input);
    Console.WriteLine();
    Print(response);

    if (response.RequestQuit)
    {
        Console.WriteLine("Farewell, adventurer.");
        break;
    }
}

static void Print(GameResponse response)
{
    foreach (var block in response.Blocks)
    {
        Console.WriteLine(block.Text);
    }
}
