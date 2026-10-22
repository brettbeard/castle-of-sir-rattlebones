using System.Runtime.CompilerServices;
using System.Text;
using CastleAdventure.Content;
using CastleAdventure.Engine;

namespace CastleAdventure.Tests.Walkthroughs;

/// <summary>
/// Plays scripted command files (<c>*.commands.txt</c>) through the engine. The win script's full output is
/// compared with <c>win.transcript.txt</c>; set UPDATE_GOLDEN=1 to regenerate it after an intended text change.
/// On a mismatch the actual output is written to <c>win.received.txt</c> for diffing.
/// </summary>
[TestClass]
public sealed class WalkthroughTests
{
    private const int Seed = 1;

    [TestMethod]
    public void Win_ReachesWinState()
    {
        var (engine, _) = Play("win.commands.txt");

        Assert.IsTrue(engine.State.HasWon);
        Assert.AreEqual(14, engine.State.Moves);
        Assert.AreEqual(0, engine.State.HintsUsed);
        Assert.IsTrue(engine.State.IsInInventory(ItemIds.SilverKey));
        Assert.IsTrue(engine.State.IsInInventory(ItemIds.GoldKey));
        Assert.AreEqual(RoomIds.Treasury, engine.State.CurrentRoomId);
    }

    [TestMethod]
    public void Win_MatchesGoldenTranscript()
    {
        var (_, transcript) = Play("win.commands.txt");
        var goldenPath = Path.Combine(Folder(), "win.transcript.txt");

        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1" || !File.Exists(goldenPath))
        {
            File.WriteAllText(goldenPath, transcript);
            return;
        }

        var expected = Normalize(File.ReadAllText(goldenPath));
        if (expected != transcript)
        {
            File.WriteAllText(Path.Combine(Folder(), "win.received.txt"), transcript);
            Assert.Fail($"Transcript differs from win.transcript.txt. {FirstDifference(expected, transcript)} " +
                "See win.received.txt; if the change is intended, rerun with UPDATE_GOLDEN=1.");
        }
    }

    [TestMethod]
    public void WrongOrder_NeverWinsOrGivesKeys()
    {
        var (engine, transcript) = Play("wrong-order.commands.txt");

        Assert.IsFalse(engine.State.HasWon);
        Assert.IsFalse(engine.State.IsInInventory(ItemIds.SilverKey));
        Assert.IsFalse(engine.State.IsInInventory(ItemIds.GoldKey));
        Assert.IsTrue(engine.State.IsLocked(LockIds.TreasuryDoor));
        Assert.Contains("We haven't even been introduced!", transcript);
        Assert.Contains("Hands off the hat", transcript);
    }

    private static (GameEngine Engine, string Transcript) Play(string scriptName)
    {
        var commands = File.ReadAllLines(Path.Combine(Folder(), scriptName))
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith('#'));

        // Flavor on with a fixed seed: realistic output (ambient lines included) that is still repeatable.
        var engine = new GameEngine(CastleWorld.Build(), new EngineOptions { Random = new Random(Seed) });
        var transcript = new StringBuilder().Append(engine.Start().Text).Append('\n');

        foreach (var command in commands)
        {
            var response = engine.Execute(command);
            transcript.Append('\n').Append("> ").Append(command).Append('\n').Append(response.Text).Append('\n');
        }

        return (engine, transcript.ToString());
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal);

    private static string FirstDifference(string expected, string actual)
    {
        var e = expected.Split('\n');
        var a = actual.Split('\n');
        for (var i = 0; i < Math.Max(e.Length, a.Length); i++)
        {
            var el = i < e.Length ? e[i] : "<end>";
            var al = i < a.Length ? a[i] : "<end>";
            if (el != al)
            {
                return $"Line {i + 1}: expected \"{el}\" but got \"{al}\".";
            }
        }

        return string.Empty;
    }

    /// <summary>This source file's folder, so scripts are read from (and golden files written to) the repo.</summary>
    private static string Folder([CallerFilePath] string path = "") =>
        Path.GetDirectoryName(path) ?? throw new InvalidOperationException("No caller path.");
}
