using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_CP
    {
        [Fact]
        public void TestEXEC_CP_r_n_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 3",
                    "CP A, 3",
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
        public void TestEXEC_CP_r_n_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 3",
                    "CP A, 2",
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
        public void TestEXEC_CP_r_n_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 3",
                    "CP A, 5",
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

        [Fact]
        public void TestEXEC_CP_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 22",
                    "LD B, 22",
                    "CP A, B",
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
        public void TestEXEC_CP_rr_nn_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 300",
                    "CP AB, 300",
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
        public void TestEXEC_CP_rr_nn_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 3000",
                    "CP AB, 2000",
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
        public void TestEXEC_CP_rr_nn_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 3000",
                    "CP AB, 5000",
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

        [Fact]
        public void TestEXEC_CP_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 2000",
                    "LD CD, 2000",
                    "CP AB, CD",
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
        public void TestEXEC_CP_rrr_nnn_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "CP ABC, 0xABCDEF",
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
        public void TestEXEC_CP_rrr_nnn_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "CP ABC, 0xAB0000",
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
        public void TestEXEC_CP_rrr_nnn_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "CP ABC, 0xBCDEF0",
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

        [Fact]
        public void TestEXEC_CP_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "LD DEF, 0xABCDEF",
                    "CP ABC, DEF",
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
        public void TestEXEC_CP_rrrr_nnnn_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEFAB",
                    "CP ABCD, 0xABCDEFAB",
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
        public void TestEXEC_CP_rrrr_nnnn_non_equal_lesser()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEF00",
                    "CP ABCD, 0xAB000000",
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
        public void TestEXEC_CP_rrrr_nnnn_non_equal_greater()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEF00",
                    "CP ABCD, 0xBCDEF000",
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

        [Fact]
        public void TestEXEC_CP_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEFAB",
                    "LD EFGH, 0xABCDEFAB",
                    "CP ABCD, EFGH",
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
        public void TestEXEC_CP_r_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set8bitToRAM(1000, 3);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 3",
                    "LD EFG, 1000",
                    "CP A, (EFG)",
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
        public void TestEXEC_CP_rr_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set16bitToRAM(1000, 0xABCD);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 0xABCD",
                    "LD EFG, 1000",
                    "CP AB, (EFG)",
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
        public void TestEXEC_CP_rrr_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set24bitToRAM(1000, 0xABCDEF);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "LD EFG, 1000",
                    "CP ABC, (EFG)",
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
        public void TestEXEC_CP_rrrr_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set32bitToRAM(1000, 0xABCDEFAB);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEFAB",
                    "LD EFG, 1000",
                    "CP ABCD, (EFG)",
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
        public void TestEXEC_CP_IrrrI_IrrrI_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set32bitToRAM(1000, 0xAB012345);
            computer.MEMC.Set32bitToRAM(2000, 0xABCDEFAB);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(@"
                LD ABC, 1000
                LD DEF, 2000
                CP (ABC), (DEF)
                BREAK
            ");

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
        public void TestEXEC_CP_IrrrI_IrrrI_not_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set32bitToRAM(1000, 0xAB012345);
            computer.MEMC.Set32bitToRAM(2000, 0xABCDEFAB);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(@"
                LD ABC, 1001
                LD DEF, 2001
                CP (ABC), (DEF)
                BREAK
            ");

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

        [Fact]
        public void TestEXEC_CP_IrrrI_nnn_equal()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set32bitToRAM(1000, 0xABCDEFAB);
            computer.MEMC.Set32bitToRAM(2000, 0xABCDEFAB);

            Assert.False(computer.CPU.FLAGS.GetValueByName("Z"));
            Assert.False(computer.CPU.FLAGS.GetValueByName("EQ"));

            cp.Build(@"
                LD ABC, 1000
                CP (ABC), 0xAB
                BREAK
            ");

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
    }
}
