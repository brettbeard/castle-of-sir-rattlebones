namespace CastleAdventure.Tests;

// Phase 1 placeholder proving the test project builds, references the game, and runs.
// Delete once real tests exist (Phase 2).
[TestClass]
public sealed class ScaffoldTests
{
    [TestMethod]
    public void TestProjectReferencesGameAssembly()
    {
        var assemblyName = typeof(Program).Assembly.GetName().Name;

        Assert.AreEqual("CastleAdventure", assemblyName);
    }
}
