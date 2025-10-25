using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SCP
    {
        [Fact]
        public void TestEXEC_SCP_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, -3",
                    "CP A, 3",
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
        public void TestEXEC_SCP_r_r()
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
                    "LD B, -22",
                    "SCP A, B",
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
        public void TestEXEC_CP_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 300",
                    "SCP AB, -300",
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
        public void TestEXEC_CP_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 2000",
                    "LD CD, -2000",
                    "SCP AB, CD",
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
        public void TestEXEC_CP_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 8043742",
                    "SCP ABC, -8043742",
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
        public void TestEXEC_CP_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 8043742",
                    "LD DEF, -8043742",
                    "SCP ABC, DEF",
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
        public void TestEXEC_CP_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 2077093803",
                    "SCP ABCD, -2077093803",
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
        public void TestEXEC_CP_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 2077093803",
                    "LD EFGH, -2077093803",
                    "SCP ABCD, EFGH",
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
        public void TestEXEC_CP_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set8bitToRAM(1000, TUtils.GetUnsignedByte(-3));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 3",
                    "LD EFG, 1000",
                    "SCP A, (EFG)",
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
        public void TestEXEC_CP_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set16bitToRAM(1000, TUtils.GetUnsignedShort(-31693));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 31693",
                    "LD EFG, 1000",
                    "SCP AB, (EFG)",
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
        public void TestEXEC_CP_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set24bitToRAM(1000, TUtils.GetUnsigned24BitInt(-8113647));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0x7BCDEF",
                    "LD EFG, 1000",
                    "SCP ABC, (EFG)",
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
        public void TestEXEC_CP_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set32bitToRAM(1000, TUtils.GetUnsigned32BitInt(-2077093803));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0x7BCDEFAB",
                    "LD EFG, 1000",
                    "SCP ABCD, (EFG)",
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
        public void TestEXEC_CP_IrrrI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set8bitToRAM(1000, TUtils.GetUnsignedByte(100));
            computer.MEMC.Set8bitToRAM(2000, TUtils.GetUnsignedByte(-100));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(@"
                LD ABC, 1000
                LD DEF, 2000
                SCP (ABC), (DEF)
                BREAK
            ");

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
        public void TestEXEC_CP_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            computer.Clear();
            computer.CPU.FLAGS.ResetAll();

            computer.MEMC.Set8bitToRAM(1000, TUtils.GetUnsignedByte(100));

            Assert.False(computer.CPU.FLAGS.GetValueByName("GT"));

            cp.Build(@"
                LD ABC, 1000
                SCP (ABC), -100
                BREAK
            ");

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
    }
}
