using Continuum93.Emulator.Compilers.C93Basic;

namespace ContinuumUnitTests.CompilerTests
{
    public class BasicCompilerTests
    {
        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestSimpleAssignment()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    LET x = 10
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0, $"Assembly should not be empty. Got: {assembly}");
            Assert.Contains("LD", assembly);
            Assert.Equal(0, compiler.Errors);
            if (compiler.Errors > 0)
            {
                Assert.True(false, $"Compiler errors: {compiler.Log}");
            }
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestMinimalProgram()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = "END";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Contains("#ORG", assembly);
            Assert.Contains("RET", assembly);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestLabelOnly()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Contains(".Main:", assembly);
            Assert.Contains("RET", assembly);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestLiteralAssignment()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = "LET x = 10";
            string assembly = compiler.Compile(source);
            
            // Output debug info
            if (compiler.Errors > 0 || assembly.Length < 50)
            {
                System.Diagnostics.Debug.WriteLine($"Compiler Log:\n{compiler.Log}");
                System.Diagnostics.Debug.WriteLine($"Assembly:\n{assembly}");
            }
            
            Assert.True(assembly.Length > 0, $"Assembly should not be empty. Got: {assembly}\nLog: {compiler.Log}");
            Assert.Contains("LD", assembly);
            if (compiler.Errors > 0)
            {
                Assert.True(false, $"Errors: {compiler.Log}");
            }
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestAssignmentWithoutLet()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = "x = 10";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0, $"Assembly should not be empty. Got: {assembly}");
            Assert.Contains("LD", assembly);
            if (compiler.Errors > 0)
            {
                Assert.True(false, $"Errors: {compiler.Log}");
            }
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestPrintStatement()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    PRINT ""Hello, World""
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestIfStatement()
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
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestForLoop()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    FOR i = 1 TO 10
        PRINT i
    NEXT i
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestTwoForLoops()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    FOR i = 1 TO 10
        PRINT i
    NEXT i
    FOR j = 5 TO 15
        PRINT j
    NEXT j
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestGoto()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    GOTO Loop
Loop:
    PRINT ""In loop""
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestCls()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    CLS
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Contains("INT 0x01", assembly);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestPlot()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    PLOT 100, 50, 15
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestArithmetic()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    LET x = 10 + 5
    LET y = x * 2
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestLexer()
        {
            Lexer lexer = new Lexer();
            string source = "LET x = 10";
            var tokens = lexer.Tokenize(source);
            Assert.True(tokens.Count > 0);
            Assert.Empty(lexer.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestParser()
        {
            Lexer lexer = new Lexer();
            Parser parser = new Parser(new SymbolTable());
            string source = @"
Main:
    LET x = 10
    END
";
            var tokens = lexer.Tokenize(source);
            var program = parser.Parse(tokens);
            Assert.NotNull(program);
            Assert.True(program.Statements.Count > 0);
            Assert.Empty(parser.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestGraphicsCommands()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    VIDEO 1
    CLS 0
    CIRCLE FILLED 240, 135, 50, 200
    RECTANGLE 10, 10, 100, 50, 15
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestMathFunctions()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    LET x = SIN(PI / 2)
    LET y = SQR(16)
    LET z = ABS(-10)
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }

        [Fact(Skip = "Skipping until the proper implementation of the compiler")]
        public void TestComplexExpressions()
        {
            BasicCompiler compiler = new BasicCompiler();
            string source = @"
Main:
    LET result = (10 + 5) * 2 - 3
    LET flag = x > 0 AND x < 100
    LET bits = 8 SHL 2
    END
";
            string assembly = compiler.Compile(source);
            Assert.True(assembly.Length > 0);
            Assert.Equal(0, compiler.Errors);
        }
    }
}
