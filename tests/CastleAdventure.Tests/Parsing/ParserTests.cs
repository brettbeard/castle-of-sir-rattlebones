using CastleAdventure.Model;
using CastleAdventure.Parsing;

namespace CastleAdventure.Tests.Parsing;

[TestClass]
public sealed class ParserTests
{
    // ---- Empty and unknown input ----

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\t")]
    [DataRow("the")]
    [DataRow("...")]
    public void EmptyOrMeaninglessInput_IsNone(string input)
    {
        Assert.AreEqual(Verb.None, Parser.Parse(input).Verb);
    }

    [TestMethod]
    public void NullInput_IsNone()
    {
        Assert.AreEqual(Verb.None, Parser.Parse(null).Verb);
    }

    [TestMethod]
    [DataRow("xyzzy", "xyzzy")]
    [DataRow("XYZZY the lamp", "xyzzy")]
    [DataRow("dance with skeleton", "dance")]
    [DataRow("pick diary", "pick")]
    public void UnknownFirstWord_IsUnknown_AndReportsTheWord(string input, string expectedWord)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(Verb.Unknown, command.Verb);
        Assert.AreEqual(expectedWord, command.UnknownWord);
    }

    // ---- Movement ----

    [TestMethod]
    [DataRow("north", Direction.North)]
    [DataRow("n", Direction.North)]
    [DataRow("south", Direction.South)]
    [DataRow("s", Direction.South)]
    [DataRow("east", Direction.East)]
    [DataRow("e", Direction.East)]
    [DataRow("west", Direction.West)]
    [DataRow("w", Direction.West)]
    [DataRow("up", Direction.Up)]
    [DataRow("u", Direction.Up)]
    [DataRow("down", Direction.Down)]
    [DataRow("d", Direction.Down)]
    [DataRow("go north", Direction.North)]
    [DataRow("go n", Direction.North)]
    [DataRow("walk east", Direction.East)]
    [DataRow("climb down", Direction.Down)]
    [DataRow("climb down the steps", Direction.Down)]
    [DataRow("GO   WEST", Direction.West)]
    [DataRow("  N  ", Direction.North)]
    [DataRow("North.", Direction.North)]
    public void Directions_ParseAsMove(string input, Direction expected)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(Verb.Move, command.Verb);
        Assert.AreEqual(expected, command.Direction);
    }

    [TestMethod]
    public void GoWithoutDirection_IsMoveWithNoDirection()
    {
        var command = Parser.Parse("go");

        Assert.AreEqual(Verb.Move, command.Verb);
        Assert.IsNull(command.Direction);
        Assert.IsNull(command.DirectObject);
    }

    [TestMethod]
    public void GoToPlace_IsMoveWithObjectButNoDirection()
    {
        var command = Parser.Parse("go to the kitchen");

        Assert.AreEqual(Verb.Move, command.Verb);
        Assert.IsNull(command.Direction);
        Assert.AreEqual("to kitchen", command.DirectObject);
    }

    // ---- Verb synonyms ----

    [TestMethod]
    [DataRow("look", Verb.Look)]
    [DataRow("l", Verb.Look)]
    [DataRow("look around", Verb.Look)]
    [DataRow("inventory", Verb.Inventory)]
    [DataRow("inv", Verb.Inventory)]
    [DataRow("i", Verb.Inventory)]
    [DataRow("help", Verb.Help)]
    [DataRow("?", Verb.Help)]
    [DataRow("hint", Verb.Hint)]
    [DataRow("hints", Verb.Hint)]
    [DataRow("restart", Verb.Restart)]
    [DataRow("quit", Verb.Quit)]
    [DataRow("q", Verb.Quit)]
    [DataRow("exit", Verb.Quit)]
    [DataRow("talk", Verb.Talk)]
    public void SingleWordCommands(string input, Verb expected)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(expected, command.Verb);
        Assert.IsNull(command.DirectObject);
    }

    [TestMethod]
    [DataRow("examine altar", Verb.Examine)]
    [DataRow("x altar", Verb.Examine)]
    [DataRow("inspect altar", Verb.Examine)]
    [DataRow("look at altar", Verb.Examine)]
    [DataRow("look altar", Verb.Examine)]
    [DataRow("l altar", Verb.Examine)]
    [DataRow("read altar", Verb.Read)]
    [DataRow("take altar", Verb.Take)]
    [DataRow("get altar", Verb.Take)]
    [DataRow("grab altar", Verb.Take)]
    [DataRow("pick up altar", Verb.Take)]
    [DataRow("drop altar", Verb.Drop)]
    [DataRow("discard altar", Verb.Drop)]
    [DataRow("put down altar", Verb.Drop)]
    [DataRow("open altar", Verb.Open)]
    [DataRow("unlock altar", Verb.Unlock)]
    [DataRow("use altar", Verb.Use)]
    [DataRow("lift altar", Verb.Search)]
    [DataRow("move altar", Verb.Search)]
    [DataRow("raise altar", Verb.Search)]
    [DataRow("search altar", Verb.Search)]
    [DataRow("look under altar", Verb.Search)]
    [DataRow("look behind altar", Verb.Search)]
    [DataRow("look in altar", Verb.Search)]
    [DataRow("talk to altar", Verb.Talk)]
    [DataRow("talk with altar", Verb.Talk)]
    [DataRow("talk altar", Verb.Talk)]
    [DataRow("speak to altar", Verb.Talk)]
    [DataRow("speak with altar", Verb.Talk)]
    [DataRow("greet altar", Verb.Talk)]
    public void VerbSynonyms_WithObject(string input, Verb expected)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(expected, command.Verb);
        Assert.AreEqual("altar", command.DirectObject);
        Assert.IsNull(command.Preposition);
    }

    // ---- Articles, case, whitespace, punctuation ----

    [TestMethod]
    [DataRow("take the silver key")]
    [DataRow("take a silver key")]
    [DataRow("take my silver key")]
    [DataRow("TAKE THE SILVER KEY")]
    [DataRow("Take The Silver Key")]
    [DataRow("   take    the   silver   key   ")]
    [DataRow("take the silver key.")]
    [DataRow("take the silver key!")]
    [DataRow("take, the silver key")]
    public void ArticlesCaseSpacesAndPunctuation_AreIgnored(string input)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(Verb.Take, command.Verb);
        Assert.AreEqual("silver key", command.DirectObject);
    }

    [TestMethod]
    public void HyphenatedWords_AreKept()
    {
        Assert.AreEqual("moth-eaten cloth", Parser.Parse("lift the moth-eaten cloth").DirectObject);
    }

    [TestMethod]
    public void PossessiveS_IsRemoved()
    {
        Assert.AreEqual("skeleton hat", Parser.Parse("examine the skeleton's hat").DirectObject);
    }

    // ---- Prepositions and two-object commands ----

    [TestMethod]
    [DataRow("unlock door with key", Verb.Unlock, "door", "with", "key")]
    [DataRow("unlock the door with the silver key", Verb.Unlock, "door", "with", "silver key")]
    [DataRow("unlock door using key", Verb.Unlock, "door", "with", "key")]
    [DataRow("open chest with gold key", Verb.Open, "chest", "with", "gold key")]
    [DataRow("use key on door", Verb.Use, "key", "on", "door")]
    [DataRow("use the gold key on the treasure chest", Verb.Use, "gold key", "on", "treasure chest")]
    [DataRow("use key onto chest", Verb.Use, "key", "on", "chest")]
    [DataRow("ask skeleton about key", Verb.Ask, "skeleton", "about", "key")]
    [DataRow("ask the skeleton about the gold key", Verb.Ask, "skeleton", "about", "gold key")]
    [DataRow("ask sir rattlebones about hat", Verb.Ask, "sir rattlebones", "about", "hat")]
    [DataRow("ASK SKELETON ABOUT KEY", Verb.Ask, "skeleton", "about", "key")]
    [DataRow("ask skeleton for the key", Verb.Ask, "skeleton", "about", "key")]
    public void Prepositions_SplitDirectAndIndirectObjects(
        string input, Verb verb, string direct, string preposition, string indirect)
    {
        var command = Parser.Parse(input);

        Assert.AreEqual(verb, command.Verb);
        Assert.AreEqual(direct, command.DirectObject);
        Assert.AreEqual(preposition, command.Preposition);
        Assert.AreEqual(indirect, command.IndirectObject);
    }

    [TestMethod]
    public void AskAboutWithoutCharacter_HasNoDirectObject()
    {
        var command = Parser.Parse("ask about the key");

        Assert.AreEqual(Verb.Ask, command.Verb);
        Assert.IsNull(command.DirectObject);
        Assert.AreEqual("about", command.Preposition);
        Assert.AreEqual("key", command.IndirectObject);
    }

    [TestMethod]
    public void PrepositionWithNothingAfter_HasNullIndirectObject()
    {
        var command = Parser.Parse("unlock door with");

        Assert.AreEqual("door", command.DirectObject);
        Assert.AreEqual("with", command.Preposition);
        Assert.IsNull(command.IndirectObject);
    }

    [TestMethod]
    public void VerbWithoutObject_HasNullObjects()
    {
        var command = Parser.Parse("take");

        Assert.AreEqual(Verb.Take, command.Verb);
        Assert.IsNull(command.DirectObject);
        Assert.IsNull(command.IndirectObject);
    }

    [TestMethod]
    public void LookAtWithoutObject_IsExamineWithNoObject()
    {
        var command = Parser.Parse("look at");

        Assert.AreEqual(Verb.Examine, command.Verb);
        Assert.IsNull(command.DirectObject);
    }

    // ---- Unknown nouns are passed through for the engine to judge ----

    [TestMethod]
    public void KnownVerbWithUnknownNoun_PassesNounThrough()
    {
        var command = Parser.Parse("take the flux capacitor");

        Assert.AreEqual(Verb.Take, command.Verb);
        Assert.AreEqual("flux capacitor", command.DirectObject);
    }

    // ---- Vocabulary hygiene ----

    [TestMethod]
    public void Vocabulary_AllEntriesAreLowercase()
    {
        var words = Vocabulary.VerbPhrases.Keys
            .Concat(Vocabulary.Directions.Keys)
            .Concat(Vocabulary.Prepositions.Keys)
            .Concat(Vocabulary.Stopwords);

        foreach (var word in words)
        {
            Assert.AreEqual(word.ToLowerInvariant(), word);
        }
    }

    [TestMethod]
    public void Vocabulary_DirectionsDoNotCollideWithVerbs()
    {
        foreach (var direction in Vocabulary.Directions.Keys)
        {
            Assert.IsFalse(Vocabulary.VerbPhrases.ContainsKey(direction), direction);
        }
    }
}
