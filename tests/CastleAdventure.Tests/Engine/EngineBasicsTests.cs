using CastleAdventure.Engine;

namespace CastleAdventure.Tests.Engine;

[TestClass]
public sealed class EngineBasicsTests
{
    [TestMethod]
    public void EmptyInput_BegPardon_NotCounted()
    {
        var response = new TestGame().Do("   ");

        Assert.AreEqual("Beg pardon?", response.Text);
        Assert.IsFalse(response.CountedMove);
    }

    [TestMethod]
    public void UnknownVerb_ReportsWord_NotCounted()
    {
        var response = new TestGame().Do("xyzzy");

        Assert.AreEqual("I don't know the word 'xyzzy'.", response.Text);
        Assert.IsFalse(response.CountedMove);
    }

    [TestMethod]
    public void Quit_RequestsQuit()
    {
        var response = new TestGame().Do("quit");

        Assert.IsTrue(response.RequestQuit);
        Assert.IsFalse(response.CountedMove);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("xyzzy")]
    [DataRow("take")]
    [DataRow("drop the")]
    [DataRow("go to the moon")]
    [DataRow("open")]
    [DataRow("ask about")]
    [DataRow("unlock with")]
    [DataRow("!!!")]
    [DataRow("x x x x x")]
    public void OddInput_NeverThrows(string input)
    {
        var response = new TestGame().Do(input);

        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Text));
    }

    [TestMethod]
    [DataRow(new[] { "a" }, "a")]
    [DataRow(new[] { "a", "b" }, "a and b")]
    [DataRow(new[] { "a", "b", "c" }, "a, b and c")]
    public void Prose_JoinAnd(string[] items, string expected)
    {
        Assert.AreEqual(expected, Prose.JoinAnd(items));
    }
}
