using CastleAdventure.Output;

namespace CastleAdventure.Tests.Output;

[TestClass]
public sealed class TextWrapperTests
{
    [TestMethod]
    public void ShortLine_Unchanged()
    {
        CollectionAssert.AreEqual(new[] { "Taken." }, TextWrapper.Wrap("Taken.", 20).ToList());
    }

    [TestMethod]
    public void WrapsAtWordBoundaries()
    {
        var lines = TextWrapper.Wrap("the quick brown fox jumps over the lazy dog", 15);

        CollectionAssert.AreEqual(new[] { "the quick brown", "fox jumps over", "the lazy dog" }, lines.ToList());
    }

    [TestMethod]
    public void NoLineExceedsWidth_ForRealRoomText()
    {
        var text = new Engine.TestGame().Do("look").Text;

        foreach (var width in new[] { 79, 60, 40, 20 })
        {
            foreach (var line in TextWrapper.Wrap(text, width))
            {
                Assert.IsLessThanOrEqualTo(width, line.Length, $"width {width}: \"{line}\"");
            }
        }
    }

    [TestMethod]
    public void KeepsExistingLineBreaksAndBlankLines()
    {
        var lines = TextWrapper.Wrap("one\n\ntwo", 20);

        CollectionAssert.AreEqual(new[] { "one", string.Empty, "two" }, lines.ToList());
    }

    [TestMethod]
    public void NormalisesWindowsLineEndings()
    {
        CollectionAssert.AreEqual(new[] { "a", "b" }, TextWrapper.Wrap("a\r\nb", 20).ToList());
    }

    [TestMethod]
    public void KeepsIndentOnContinuationLines()
    {
        var lines = TextWrapper.Wrap("  a rusty pot and a dusty diary", 14);

        CollectionAssert.AreEqual(new[] { "  a rusty pot", "  and a dusty", "  diary" }, lines.ToList());
    }

    [TestMethod]
    public void SplitsWordsLongerThanWidth()
    {
        var lines = TextWrapper.Wrap("go abcdefghijklmnopqrstuvwxyz now", 10);

        CollectionAssert.AreEqual(new[] { "go", "abcdefghij", "klmnopqrst", "uvwxyz now" }, lines.ToList());
    }

    [TestMethod]
    public void CollapsesRepeatedSpaces()
    {
        CollectionAssert.AreEqual(new[] { "a b" }, TextWrapper.Wrap("a    b", 20).ToList());
    }

    [TestMethod]
    public void TinyWidth_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TextWrapper.Wrap("text", 5));
    }
}
