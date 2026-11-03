namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>
    /// The C compiler and OS tests run on their own (not in parallel with other test classes): the emulator keeps
    /// some state in statics (keyboard, compile log), and isolating these long-running tests keeps both sides stable.
    /// </summary>
    [CollectionDefinition("CCompiler", DisableParallelization = true)]
    public class CCompilerCollection
    {
    }
}
