using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_CP_float
    {
        // CP fr, fr
        [Fact]
        public void TestEXEC_CP_fr_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.14",
                    "LD F1, 3.14",
                    "CP F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD F1, 2",
                    "CP F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD F1, 5.0",
                    "CP F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, nnn
        [Fact]
        public void TestEXEC_CP_fr_nnn_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.14",
                    "CP F0, 3.14",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_nnn_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP F0, 2",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_nnn_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP F0, 5.0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, r
        [Fact]
        public void TestEXEC_CP_fr_r_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD A, 3",
                    "CP F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_r_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD A, 2",
                    "CP F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_r_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD A, 5",
                    "CP F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, rr
        [Fact]
        public void TestEXEC_CP_fr_rr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD AB, 3",
                    "CP F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD AB, 2",
                    "CP F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD AB, 5",
                    "CP F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, rrr
        [Fact]
        public void TestEXEC_CP_fr_rrr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 3",
                    "CP F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rrr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 2",
                    "CP F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rrr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 5",
                    "CP F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, rrrr
        [Fact]
        public void TestEXEC_CP_fr_rrrr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABCD, 3",
                    "CP F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rrrr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABCD, 2",
                    "CP F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_rrrr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABCD, 5",
                    "CP F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP r, fr
        [Fact]
        public void TestEXEC_CP_r_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD A, 3",
                    "CP A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_r_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "LD A, 3",
                    "CP A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_r_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "LD A, 3",
                    "CP A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP rr, fr
        [Fact]
        public void TestEXEC_CP_rr_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD AB, 3",
                    "CP AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rr_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "LD AB, 3",
                    "CP AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rr_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "LD AB, 3",
                    "CP AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP rrr, fr
        [Fact]
        public void TestEXEC_CP_rrr_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 3",
                    "CP ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rrr_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "LD ABC, 3",
                    "CP ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rrr_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "LD ABC, 3",
                    "CP ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP rrrr, fr
        [Fact]
        public void TestEXEC_CP_rrrr_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABCD, 3",
                    "CP ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rrrr_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "LD ABCD, 3",
                    "CP ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_rrrr_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "LD ABCD, 3",
                    "CP ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, (nnn)
        [Fact]
        public void TestEXEC_CP_fr_InnnI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_InnnI_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 2.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_InnnI_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 5.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP fr, (rrr)
        [Fact]
        public void TestEXEC_CP_fr_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 0x2000",
                    "CP F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_IrrrI_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 2.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 0x2000",
                    "CP F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_fr_IrrrI_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 5.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 0x2000",
                    "CP F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP (nnn), fr
        [Fact]
        public void TestEXEC_CP_InnnI_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "CP (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_InnnI_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "CP (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_InnnI_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "CP (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        // CP (rrr), fr
        [Fact]
        public void TestEXEC_CP_IrrrI_fr_equal()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.0",
                    "LD ABC, 0x2000",
                    "CP (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.True(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_IrrrI_fr_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 2.0",
                    "LD ABC, 0x2000",
                    "CP (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CP_IrrrI_fr_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.MEMC.SetFloatToRam(0x2000, 3.0f);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 5.0",
                    "LD ABC, 0x2000",
                    "CP (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Equality tests
            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LT"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("GTE"));
            Assert.True(computer.CPU.FLAGS.GetValueByName("LTE"));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
