using Continuum93.Emulator.Compilers.C93Basic;
using System.IO;

namespace ContinuumUnitTests.CompilerTests
{
    public class BasicCompilerDebugTests
    {
        [Fact]
        public void TestIfStatementDebug()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    IF x > 10 THEN
        PRINT ""Large""
    END IF
    END
";
            string assembly = compiler.Compile(source);
            
            // Write log to file for debugging
            string debugPath = Path.Combine(Path.GetTempPath(), "basic_if_debug.txt");
            File.WriteAllText(debugPath, $"=== COMPILER LOG ===\n{compiler.Log}\n=== ASSEMBLY ===\n{assembly}\n===================\nErrors: {compiler.Errors}\n");
            
            Assert.True(assembly.Length > 0, $"Assembly empty. Log written to: {debugPath}");
            Assert.Equal(0, compiler.Errors);
        }
    }
}
