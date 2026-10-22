using CastleAdventure.Engine;

namespace CastleAdventure.Output;

/// <summary>
/// All console input and output: wrapping, colours, prompts. Colour is skipped when output is
/// redirected, and key waits are skipped when input is redirected (e.g. a piped walkthrough).
/// </summary>
public sealed class ConsoleUi
{
    private readonly bool _useColor = !Console.IsOutputRedirected;

    public int Width { get; } = DetectWidth();

    public void Print(GameResponse response)
    {
        foreach (var block in response.Blocks)
        {
            Print(block.Text, block.Style);
        }
    }

    public void Print(string text, TextStyle style = TextStyle.Normal)
    {
        var color = style switch
        {
            TextStyle.RoomName => ConsoleColor.Yellow,
            TextStyle.Victory => ConsoleColor.Green,
            _ => (ConsoleColor?)null,
        };

        if (_useColor && color is { } c)
        {
            Console.ForegroundColor = c;
        }

        foreach (var line in TextWrapper.Wrap(text, Width))
        {
            Console.WriteLine(line);
        }

        if (_useColor && color is not null)
        {
            Console.ResetColor();
        }
    }

    /// <summary>Prints verbatim, without wrapping (for ASCII art).</summary>
    public static void PrintRaw(string text) => Console.WriteLine(text);

    public static void BlankLine() => Console.WriteLine();

    /// <summary>Shows the prompt and reads a line; null at end of input.</summary>
    public static string? Prompt()
    {
        Console.WriteLine();
        Console.Write("> ");
        return Console.ReadLine();
    }

    /// <summary>Asks a Y/N question. End of input counts as <paramref name="whenNoInput"/>.</summary>
    public bool Confirm(string question, bool whenNoInput)
    {
        while (true)
        {
            BlankLine();
            Print(question);
            Console.Write("> ");
            var answer = Console.ReadLine()?.Trim().ToLowerInvariant();
            switch (answer)
            {
                case null:
                    return whenNoInput;
                case "y" or "yes":
                    return true;
                case "n" or "no":
                    return false;
            }
        }
    }

    public static void WaitForKey()
    {
        if (!Console.IsInputRedirected)
        {
            Console.ReadKey(intercept: true);
        }
    }

    private static int DetectWidth()
    {
        try
        {
            var window = Console.IsOutputRedirected ? 0 : Console.WindowWidth;
            return window > 0 ? Math.Min(window, TextWrapper.DefaultWidth + 1) - 1 : TextWrapper.DefaultWidth;
        }
        catch (IOException)
        {
            return TextWrapper.DefaultWidth;
        }
    }
}
