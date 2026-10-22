using System.Text;

namespace CastleAdventure.Output;

/// <summary>
/// Word-wraps text to a column width. Existing line breaks are kept; a line's leading spaces are
/// kept and repeated on its continuation lines; words longer than the width are split.
/// </summary>
public static class TextWrapper
{
    public const int DefaultWidth = 79;

    public static IReadOnlyList<string> Wrap(string text, int width)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 10);

        var result = new List<string>();
        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            WrapLine(line, width, result);
        }

        return result;
    }

    private static void WrapLine(string line, int width, List<string> result)
    {
        var indent = line[..(line.Length - line.TrimStart(' ').Length)];
        var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            result.Add(string.Empty);
            return;
        }

        var current = new StringBuilder(indent);
        var hasWord = false;
        foreach (var word in words)
        {
            var remaining = word;
            while (indent.Length + remaining.Length > width)
            {
                // A single word wider than the line: flush, then split it.
                if (hasWord)
                {
                    result.Add(current.ToString());
                    current.Clear().Append(indent);
                    hasWord = false;
                }

                var take = width - indent.Length;
                result.Add(indent + remaining[..take]);
                remaining = remaining[take..];
            }

            if (remaining.Length == 0)
            {
                continue;
            }

            if (hasWord && current.Length + 1 + remaining.Length > width)
            {
                result.Add(current.ToString());
                current.Clear().Append(indent);
                hasWord = false;
            }

            current.Append(hasWord ? " " : string.Empty).Append(remaining);
            hasWord = true;
        }

        if (hasWord)
        {
            result.Add(current.ToString());
        }
    }
}
