using CastleAdventure.Model;

namespace CastleAdventure.Tests.Model;

/// <summary>Proves the validator catches each kind of broken world, so a clean castle result means something.</summary>
[TestClass]
public sealed class WorldValidatorTests
{
    [TestMethod]
    public void Validate_MinimalWorld_HasNoProblems()
    {
        var problems = WorldValidator.Validate(TestWorldBuilder.Minimal().Build());

        Assert.IsEmpty(problems, string.Join(Environment.NewLine, problems));
    }

    [TestMethod]
    public void Validate_MissingStartRoom()
    {
        AssertProblem(TestWorldBuilder.Minimal().StartIn("nowhere"), "Start room 'nowhere' does not exist");
    }

    [TestMethod]
    public void Validate_ExitToMissingRoom()
    {
        AssertProblem(TestWorldBuilder.Minimal().OneWay("hall", Direction.West, "attic"), "missing room 'attic'");
    }

    [TestMethod]
    public void Validate_OneWayExit()
    {
        var builder = TestWorldBuilder.Minimal().Room("cellar").OneWay("hall", Direction.Down, "cellar");

        AssertProblem(builder, "has no matching up exit back");
    }

    [TestMethod]
    public void Validate_UnreachableRoom()
    {
        AssertProblem(TestWorldBuilder.Minimal().Room("island"), "Room 'island' cannot be reached");
    }

    [TestMethod]
    public void Validate_LockedExitWithoutBlockedMessage()
    {
        var builder = TestWorldBuilder.Minimal().Configure("hall", r =>
            r.Exits[Direction.North] = new Exit("vault") { LockId = "missing_lock" });

        AssertProblem(builder, "uses missing lock 'missing_lock'");
        AssertProblem(builder, "is locked but has no BlockedMessage");
    }

    [TestMethod]
    public void Validate_HiddenItemNeverRevealedOrGiven()
    {
        var builder = TestWorldBuilder.Minimal().Configure("hall", r => r.Items.Add(new Item
        {
            Id = "gem",
            Name = "gem",
            Description = "A gem.",
            IsHidden = true,
        }));

        AssertProblem(builder, "Hidden item 'gem' is never revealed or given");
    }

    [TestMethod]
    public void Validate_RevealedByMissingTrigger()
    {
        var builder = TestWorldBuilder.Minimal().Configure("hall", r => r.Items.Add(new Item
        {
            Id = "gem",
            Name = "gem",
            Description = "A gem.",
            IsHidden = true,
            RevealedBy = new RevealTrigger("rug", RevealVerbs.Search, "A gem!"),
        }));

        AssertProblem(builder, "revealed by missing item 'rug'");
    }

    [TestMethod]
    public void Validate_UppercaseAlias()
    {
        var builder = TestWorldBuilder.Minimal().Configure("hall", r => r.Items.Add(new Item
        {
            Id = "lamp",
            Name = "lamp",
            Aliases = { "Brass Lamp" },
            Description = "A lamp.",
        }));

        AssertProblem(builder, "'Brass Lamp' must be lowercase");
    }

    [TestMethod]
    public void Validate_LockWithMissingKey()
    {
        var builder = TestWorldBuilder.Minimal().Configure("hall", r => r.Items.Add(new Item
        {
            Id = "gate",
            Name = "gate",
            Description = "A gate.",
            Lock = new KeyLock { Id = "gate_lock", KeyId = "no_such_key", UnlockMessage = "Click." },
        }));

        AssertProblem(builder, "needs missing key 'no_such_key'");
    }

    [TestMethod]
    public void Validate_NoWinTrigger()
    {
        var builder = new TestWorldBuilder().Room("hall");

        AssertProblem(builder, "exactly one win trigger item, found 0");
    }

    [TestMethod]
    public void Validate_CharacterDialogueUsesUndeclaredState()
    {
        var ghost = TestWorldBuilder.Character("ghost");
        ghost.Talk.Add(new DialogueRule("Boo.") { NewState = "angry" });

        AssertProblem(TestWorldBuilder.Minimal().Configure("hall", r => r.Characters.Add(ghost)), "undeclared state 'angry'");
    }

    [TestMethod]
    public void Validate_CharacterGivesItemItDoesNotHold()
    {
        var ghost = TestWorldBuilder.Character("ghost");
        ghost.Talk.Add(new DialogueRule("Take this.") { GivesItemId = "ring" });

        AssertProblem(TestWorldBuilder.Minimal().Configure("hall", r => r.Characters.Add(ghost)), "gives item 'ring' that it does not hold");
    }

    [TestMethod]
    public void Validate_TopicKeywordUsedTwice()
    {
        var ghost = TestWorldBuilder.Character("ghost");
        ghost.Topics.Add(new Topic { Id = "a", Keywords = { "moon" }, Rules = { new DialogueRule("A.") } });
        ghost.Topics.Add(new Topic { Id = "b", Keywords = { "moon" }, Rules = { new DialogueRule("B.") } });

        AssertProblem(TestWorldBuilder.Minimal().Configure("hall", r => r.Characters.Add(ghost)), "keyword 'moon' in more than one topic");
    }

    private static void AssertProblem(TestWorldBuilder builder, string expectedFragment)
    {
        var problems = WorldValidator.Validate(builder.Build());

        Assert.IsTrue(
            problems.Any(p => p.Contains(expectedFragment, StringComparison.Ordinal)),
            $"Expected a problem containing \"{expectedFragment}\". Actual:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
    }
}
