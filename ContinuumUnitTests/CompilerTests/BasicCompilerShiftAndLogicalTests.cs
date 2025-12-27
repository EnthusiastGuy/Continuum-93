using Continuum93.Emulator.Compilers.C93Basic;

namespace ContinuumUnitTests.CompilerTests
{
    public class BasicCompilerShiftAndLogicalTests
    {
        [Fact]
        public void TestShiftLeftLiteral()
        {
            string source = @"
Main:
    LET bits = 8 SHL 2
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should use SL with literal: SL ABCD, 2
            Assert.True(assembly.Contains("SL") && assembly.Contains(", 2"), 
                "Shift left should use literal value directly");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestShiftRightLiteral()
        {
            string source = @"
Main:
    LET bits = 16 SHR 2
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should use SR with literal: SR ABCD, 2
            Assert.True(assembly.Contains("SR") && assembly.Contains(", 2"), 
                "Shift right should use literal value directly");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestRotateLeftLiteral()
        {
            string source = @"
Main:
    LET bits = 8 ROL 2
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should use RL with literal: RL ABCD, 2
            Assert.True(assembly.Contains("RL") && assembly.Contains(", 2"), 
                "Rotate left should use literal value directly");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestRotateRightLiteral()
        {
            string source = @"
Main:
    LET bits = 8 ROR 2
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should use RR with literal: RR ABCD, 2
            Assert.True(assembly.Contains("RR") && assembly.Contains(", 2"), 
                "Rotate right should use literal value directly");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestShiftLeftWithVariable()
        {
            string source = @"
Main:
    LET shift = 2
    LET bits = 8 SHL shift
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should use 8-bit register for shift amount
            Assert.Contains("SL", assembly);
            // The shift amount should be in an 8-bit register (last char of 32-bit reg)
            Assert.True(assembly.Contains("SL ABCD") || assembly.Contains("SL EFGH"), 
                "Should use 8-bit register for shift amount");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestLogicalAndWithComparisons()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 AND x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have both comparisons
            Assert.True(assembly.Contains("CP") && assembly.Contains("GT"), 
                "Should have greater than comparison");
            Assert.True(assembly.Contains("CP") && assembly.Contains("LT"), 
                "Should have less than comparison");
            
            // Should have AND operation
            Assert.Contains("AND", assembly);
            
            // Should have variable x declared
            Assert.True(assembly.Contains(".var_int_x") || assembly.Contains("var_int_x"), 
                "Variable x should be declared");
            
            // Should have variable flag declared
            Assert.True(assembly.Contains(".var_int_flag") || assembly.Contains("var_int_flag"), 
                "Variable flag should be declared");
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestLogicalOrWithComparisons()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x < 0 OR x > 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have OR operation
            Assert.Contains("OR", assembly);
            
            // Should have both comparisons
            Assert.Contains("CP", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestLogicalXorWithComparisons()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 XOR x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have XOR operation
            Assert.Contains("XOR", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestComplexExpressionWithShiftsAndLogical()
        {
            string source = @"
Main:
    LET result = (10 + 5) * 2 - 3
    LET flag = x > 0 AND x < 100
    LET bits = 8 SHL 2
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Debug output
            System.Diagnostics.Debug.WriteLine("=== Assembly Output ===");
            System.Diagnostics.Debug.WriteLine(assembly);
            System.Diagnostics.Debug.WriteLine("=== End Assembly ===");
            
            // Should have arithmetic operations
            Assert.True(assembly.Contains("ADD") || assembly.Contains("MUL") || assembly.Contains("SUB"), 
                "Should have arithmetic operations");
            
            // Should have logical AND
            Assert.Contains("AND", assembly);   // "Should have AND operation for flag assignment"

            // Should have shift left with literal (should NOT load 2 into a register first)
            Assert.True(assembly.Contains("SL") && assembly.Contains(", 2"), 
                "Should have shift left with literal");
            // Should NOT have redundant LD EFGH, 2 before SL
            int slIndex = assembly.IndexOf("SL");
            if (slIndex > 0)
            {
                string beforeSL = assembly.Substring(Math.Max(0, slIndex - 50), slIndex);
                Assert.DoesNotContain("LD EFGH, 2", beforeSL); // "Should not load literal 2 into register before SL"
            }
            
            // All variables should be declared
            Assert.True(assembly.Contains("var_int_result") || assembly.Contains(".var_int_result"), 
                "Variable result should be declared");
            Assert.True(assembly.Contains("var_int_x") || assembly.Contains(".var_int_x"), 
                "Variable x should be declared (auto-declared when first used)");
            Assert.True(assembly.Contains("var_int_flag") || assembly.Contains(".var_int_flag"), 
                "Variable flag should be declared");
            Assert.True(assembly.Contains("var_int_bits") || assembly.Contains(".var_int_bits"), 
                "Variable bits should be declared");
            
            // Should have flag assignment code
            Assert.Contains("var_int_flag", assembly);    // "Flag variable should be in assembly"
            Assert.Contains("CP", assembly);    // "Should have comparison operations for flag"
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestNandOperation()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 NAND x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have NAND operation
            Assert.Contains("NAND", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestNorOperation()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 NOR x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have NOR operation
            Assert.Contains("NOR", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestXnorOperation()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 XNOR x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have XNOR operation
            Assert.Contains("XNOR", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestImplyOperation()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 IMPLY x < 100
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should have IMPLY operation
            Assert.Contains("IMPLY", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact]
        public void TestBooleanResultInAssignment()
        {
            string source = @"
Main:
    LET x = 50
    LET flag = x > 0 AND x < 100
    LET result = flag
    END
";
            BasicCompiler compiler = new BasicCompiler();
            string assembly = compiler.Compile(source);
            
            // Should compile without errors
            Assert.NotNull(assembly);
            Assert.True(assembly.Length > 0);
            
            // Should have variable assignments
            Assert.True(assembly.Contains("var_int_flag") || assembly.Contains(".var_int_flag"), 
                "Variable flag should be declared");
            Assert.True(assembly.Contains("var_int_result") || assembly.Contains(".var_int_result"), 
                "Variable result should be declared");
            Assert.Equal(0, compiler.Errors);
        }
    }
}

