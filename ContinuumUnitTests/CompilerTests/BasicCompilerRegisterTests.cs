using Continuum93.Emulator.Compilers.C93Basic;

namespace ContinuumUnitTests.CompilerTests
{
    public class BasicCompilerRegisterTests
    {
        [Fact]
        public void TestRegisterAllocationNoOverlap()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    LET x = 10 + 5
    LET y = x * 2
    END
";
            string assembly = compiler.Compile(source);
            
            // Verify we're using 32-bit registers (ABCD, EFGH, etc.) with proper spacing
            Assert.Contains("ABCD", assembly);
            Assert.Contains("EFGH", assembly);
            
            // Verify we don't have overlapping registers like ABCD and BCDE
            Assert.DoesNotContain("BCDE", assembly);
            Assert.DoesNotContain("CDEF", assembly);
            Assert.DoesNotContain("DEFG", assembly);
            
            // Verify optimization: should use ADD directly on ABCD, not LD to a new register first
            // Should see: LD ABCD, 10 then ADD ABCD, EFGH (not LD CDEF, ABCD then ADD CDEF, BCDE)
            Assert.Contains("ADD ABCD", assembly);
            Assert.DoesNotContain("LD CDEF, ABCD", assembly);
            
            Assert.Equal(0, compiler.Errors);
        }
    }
}
