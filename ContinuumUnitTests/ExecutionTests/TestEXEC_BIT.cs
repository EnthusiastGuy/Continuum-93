using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Tools;

namespace ExecutionTests
{

    public class TestEXEC_BIT
    {
        #region Helper Methods

        private static void RunTest(string asm, Action<Computer> arrange, Action<Computer> assert)
        {
            Assembler cp = new();
            using Computer computer = new();

            arrange(computer);

            cp.Build(TUtils.GetFormattedAsm(asm, "BREAK"));
            if (cp.Errors > 0)
                throw new InvalidOperationException($"Assembly failed: {cp.Log}");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            assert(computer);
            TUtils.IncrementCountedTests("exec");
        }

        #endregion

        #region BIT r,* tests

        [Fact]
        public void BIT_r_n() // Ok
        {
            RunTest(
                "BIT A,4",
                c => c.CPU.REGS.A = 0x10,
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_r() // Ok
        {
            RunTest(
                "BIT A,B",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.B = 4;
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_InnnI() // Ok
        {
            RunTest(
                "BIT A,(0x4000)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.MEMC.Set8bitToRAM(0x4000, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Innn_nnnI() // Ok
        {
            RunTest(
                "BIT A,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.MEMC.Set8bitToRAM(0x4004, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Innn_rI() // Ok
        {
            RunTest(
                "BIT A,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Innn_rrI() // Ok
        {
            RunTest(
                "BIT A,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Innn_rrrI() // Ok
        {
            RunTest(
                "BIT A,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_IrrrI() // Ok
        {
            RunTest(
                "BIT A,(QRS)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Irrr_nnnI() // Ok
        {
            RunTest(
                "BIT A,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Irrr_rI() // Ok
        {
            RunTest(
                "BIT A,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Irrr_rrI() // Ok
        {
            RunTest(
                "BIT A,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_Irrr_rrrI() // Ok
        {
            RunTest(
                "BIT A,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x04);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_r_fr() // Ok
        {
            RunTest(
                "BIT A,F0",
                c =>
                {
                    c.CPU.REGS.A = 0x10;
                    c.CPU.FREGS.SetRegister(0, 4.0f);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_nn() // Ok
        {
            RunTest(
                "BIT AB,12",
                c => c.CPU.REGS.AB = 0x1000,
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_r() // Ok
        {
            RunTest(
                "BIT AB,BC",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.BC = 12;
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_InnnI() // Ok
        {
            RunTest(
                "BIT AB,(0x4000)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set8bitToRAM(0x4000, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Innn_nnnI() // Ok
        {
            RunTest(
                "BIT AB,(0x4000+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.MEMC.Set8bitToRAM(0x4004, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Innn_rI() // Ok
        {
            RunTest(
                "BIT AB,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Innn_rrI() // Ok
        {
            RunTest(
                "BIT AB,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Innn_rrrI() // Ok
        {
            RunTest(
                "BIT AB,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_IrrrI() // Ok
        {
            RunTest(
                "BIT AB,(QRS)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Irrr_nnnI() // Ok
        {
            RunTest(
                "BIT AB,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Irrr_rI() // Ok
        {
            RunTest(
                "BIT AB,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Irrr_rrI() // Ok
        {
            RunTest(
                "BIT AB,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_Irrr_rrrI() // Ok
        {
            RunTest(
                "BIT AB,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 12);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rr_fr() // Ok
        {
            RunTest(
                "BIT AB,F0",
                c =>
                {
                    c.CPU.REGS.AB = 0x1000;
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_nnn() // Ok
        {
            RunTest(
                "BIT ABC,20",
                c => c.CPU.REGS.ABC = 0x100000,
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_r() // Ok
        {
            RunTest(
                "BIT ABC,BCD",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.BCD = 20;
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_InnnI() // Ok
        {
            RunTest(
                "BIT ABC,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.MEMC.Set8bitToRAM(0x4000, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Innn_nnnI() // Ok
        {
            RunTest(
                "BIT ABC,(0x4000+16)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.MEMC.Set8bitToRAM(0x4010, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Innn_rI() // Ok
        {
            RunTest(
                "BIT ABC,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Innn_rrI() // Ok
        {
            RunTest(
                "BIT ABC,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Innn_rrrI() // Ok
        {
            RunTest(
                "BIT ABC,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_IrrrI() // Ok
        {
            RunTest(
                "BIT ABC,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "BIT ABC,(QRS+16)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5010, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Irrr_rI() // Ok
        {
            RunTest(
                "BIT ABC,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Irrr_rrI() // Ok
        {
            RunTest(
                "BIT ABC,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "BIT ABC,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 20);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrr_fr() // Ok
        {
            RunTest(
                "BIT ABC,F0",
                c =>
                {
                    c.CPU.REGS.ABC = 0x100000;
                    c.CPU.FREGS.SetRegister(0, 20.0f);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_nn() // Ok
        {
            RunTest(
                "BIT ABCD,28",
                c => c.CPU.REGS.ABCD = 0x10000000,
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_r() // Ok
        {
            RunTest(
                "BIT ABCD,EFGH",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.EFGH = 28;
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_InnnI() // Ok
        {
            RunTest(
                "BIT ABCD,(0x4000)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.MEMC.Set8bitToRAM(0x4000, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Innn_nnnI() // Ok
        {
            RunTest(
                "BIT ABCD,(0x4000+24)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.MEMC.Set8bitToRAM(0x4018, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Innn_rI() // Ok
        {
            RunTest(
                "BIT ABCD,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Innn_rrI() // Ok
        {
            RunTest(
                "BIT ABCD,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Innn_rrrI() // Ok
        {
            RunTest(
                "BIT ABCD,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_IrrrI() // Ok
        {
            RunTest(
                "BIT ABCD,(QRS)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Irrr_nnnI() // Ok
        {
            RunTest(
                "BIT ABCD,(QRS+24)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5018, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Irrr_rI() // Ok
        {
            RunTest(
                "BIT ABCD,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Irrr_rrI() // Ok
        {
            RunTest(
                "BIT ABCD,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_Irrr_rrrI() // Ok
        {
            RunTest(
                "BIT ABCD,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 28);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_rrrr_fr() // Ok
        {
            RunTest(
                "BIT ABCD,F0",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x10000000;
                    c.CPU.FREGS.SetRegister(0, 28.0f);
                },
                c => Assert.True(c.CPU.FLAGS.IsZero()));
        }

        #endregion

        #region Memory BIT tests

        [Fact]
        public void BIT_mem_InnnI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (0x4000)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (0x4000)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_r() // Ok
        {
            RunTest(
                "BIT (0x4000),A",
                c =>
                {
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_rr() // Ok
        {
            RunTest(
                "BIT (0x4000),AB",
                c =>
                {
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_rrr() // Ok
        {
            RunTest(
                "BIT (0x4000),ABC",
                c =>
                {
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_rrrr() // Ok
        {
            RunTest(
                "BIT (0x4000),ABCD",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (0x5000)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (0x5000+3)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (0x5000+X)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_InnnI_fr() // Ok
        {
            RunTest(
                "BIT (0x4000),F0",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (0x4000+4)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (0x4000+4)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_r() // Ok
        {
            RunTest(
                "BIT (0x4000+4),A",
                c =>
                {
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_rr() // Ok
        {
            RunTest(
                "BIT (0x4000+4),AB",
                c =>
                {
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_rrr() // Ok
        {
            RunTest(
                "BIT (0x4000+4),ABC",
                c =>
                {
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_rrrr() // Ok
        {
            RunTest(
                "BIT (0x4000+4),ABCD",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000+3)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Innn_nnnI_block_larger() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000+10)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x500A, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000+X)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+4), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_nnnI_fr() // Ok
        {
            RunTest(
                "BIT (0x4000+4),F0",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (0x4000+X)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (0x4000+X)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_r() // Ok
        {
            RunTest(
                "BIT (0x4000+X),A",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_rr() // Ok
        {
            RunTest(
                "BIT (0x4000+X),AB",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_rrr() // Ok
        {
            RunTest(
                "BIT (0x4000+X),ABC",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_rrrr() // Ok
        {
            RunTest(
                "BIT (0x4000+X),ABCD",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (0x5000)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (QRS)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+X), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rI_fr() // Ok
        {
            RunTest(
                "BIT (0x4000+X),F0",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_r() // Ok
        {
            RunTest(
                "BIT (0x4000+WX),A",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_rr() // Ok
        {
            RunTest(
                "BIT (0x4000+WX),AB",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_rrr() // Ok
        {
            RunTest(
                "BIT (0x4000+WX),ABC",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_rrrr() // Ok
        {
            RunTest(
                "BIT (0x4000+WX),ABCD",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (0x5000)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (QRS)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+WX), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrI_fr() // Ok
        {
            RunTest(
                "BIT (0x4000+WX),F0",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_r() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX),A",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_rr() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX),AB",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_rrr() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX),ABC",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_rrrr() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX),ABCD",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (0x5000)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (QRS)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Innn_rrrI_fr() // Ok
        {
            RunTest(
                "BIT (0x4000+VWX),F0",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_r() // Ok
        {
            RunTest(
                "BIT (QRS),A",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_rr() // Ok
        {
            RunTest(
                "BIT (QRS),AB",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_rrr() // Ok
        {
            RunTest(
                "BIT (QRS),ABC",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_rrrr() // Ok
        {
            RunTest(
                "BIT (QRS),ABCD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (0x5000)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (0x5000+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_IrrrI_fr() // Ok
        {
            RunTest(
                "BIT (QRS),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (QRS+4)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (QRS+4)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_r() // Ok
        {
            RunTest(
                "BIT (QRS+4),A",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_rr() // Ok
        {
            RunTest(
                "BIT (QRS+4),AB",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_rrr() // Ok
        {
            RunTest(
                "BIT (QRS+4),ABC",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_rrrr() // Ok
        {
            RunTest(
                "BIT (QRS+4),ABCD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (0x5000)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (0x5000+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+4), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_nnnI_fr() // Ok
        {
            RunTest(
                "BIT (QRS+4),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_r() // Ok
        {
            RunTest(
                "BIT (QRS+X),A",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_rr() // Ok
        {
            RunTest(
                "BIT (QRS+X),AB",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_rrr() // Ok
        {
            RunTest(
                "BIT (QRS+X),ABC",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_rrrr() // Ok
        {
            RunTest(
                "BIT (QRS+X),ABCD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (0x5000)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+X), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rI_fr() // Ok
        {
            RunTest(
                "BIT (QRS+X),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_r() // Ok
        {
            RunTest(
                "BIT (QRS+WX),A",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_rr() // Ok
        {
            RunTest(
                "BIT (QRS+WX),AB",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_rrr() // Ok
        {
            RunTest(
                "BIT (QRS+WX),ABC",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_rrrr() // Ok
        {
            RunTest(
                "BIT (QRS+WX),ABCD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (0x5000)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+WX), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrI_fr() // Ok
        {
            RunTest(
                "BIT (QRS+WX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_block_immediate_2bytes_once() // Ok
        {
            RunTest(
                "BIT (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_block_immediate_2bytes_twice() // Ok
        {
            RunTest(
                "BIT (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_r() // Ok
        {
            RunTest(
                "BIT (QRS+VWX),A",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.A = 0x08;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_rr() // Ok
        {
            RunTest(
                "BIT (QRS+VWX),AB",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.AB = 0x0800;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_rrr() // Ok
        {
            RunTest(
                "BIT (QRS+VWX),ABC",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.ABC = 0x00080000;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_rrrr() // Ok
        {
            RunTest(
                "BIT (QRS+VWX),ABCD",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.ABCD = 0x08000000;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_InnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (0x5000)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Innn_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (0x5000+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Innn_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (0x5000+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Innn_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (0x5000+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Innn_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (0x5000+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_IrrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Irrr_nnnI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (QRS+3)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Irrr_rI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (QRS+Y)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.Y = 5;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Irrr_rrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_value_Irrr_rrrI_block() // Ok
        {
            RunTest(
                "BIT (QRS+VWX), (QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_mem_Irrr_rrrI_fr() // Ok
        {
            RunTest(
                "BIT (QRS+VWX),F0",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.CPU.FREGS.SetRegister(0, 3.0f);
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        #endregion

        #region Float Register BIT tests

        [Fact]
        public void BIT_fr_fr() // Ok
        {
            RunTest(
                "BIT F0,F1",
                c =>
                {
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                    c.CPU.FREGS.SetRegister(1, 3.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_nn() // Ok
        {
            RunTest(
                "BIT F0,4",
                c => c.CPU.FREGS.SetRegister(0, 12.0f),
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_r() // Ok
        {
            RunTest(
                "BIT F0,A",
                c =>
                {
                    c.CPU.REGS.A = 4;
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_rr() // Ok
        {
            RunTest(
                "BIT F0,AB",
                c =>
                {
                    c.CPU.REGS.AB = 0x0004;
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_rrr() // Ok
        {
            RunTest(
                "BIT F0,ABC",
                c =>
                {
                    c.CPU.REGS.ABC = 0x000004;
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_rrrr() // Ok
        {
            RunTest(
                "BIT F0,ABCD",
                c =>
                {
                    c.CPU.REGS.ABCD = 0x00000004;
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_InnnI() // Ok
        {
            RunTest(
                "BIT F0,(0x4000)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4000, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Innn_nnnI() // Ok
        {
            RunTest(
                "BIT F0,(0x4000+4)",
                c =>
                {
                    c.MEMC.Set8bitToRAM(0x4004, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Innn_rI() // Ok
        {
            RunTest(
                "BIT F0,(0x4000+X)",
                c =>
                {
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x4003, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Innn_rrI() // Ok
        {
            RunTest(
                "BIT F0,(0x4000+WX)",
                c =>
                {
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x4005, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Innn_rrrI() // Ok
        {
            RunTest(
                "BIT F0,(0x4000+VWX)",
                c =>
                {
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x4007, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_IrrrI() // Ok
        {
            RunTest(
                "BIT F0,(QRS)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5000, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Irrr_nnnI() // Ok
        {
            RunTest(
                "BIT F0,(QRS+4)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.MEMC.Set8bitToRAM(0x5004, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Irrr_rI() // Ok
        {
            RunTest(
                "BIT F0,(QRS+X)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.X = 3;
                    c.MEMC.Set8bitToRAM(0x5003, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Irrr_rrI() // Ok
        {
            RunTest(
                "BIT F0,(QRS+WX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.WX = 5;
                    c.MEMC.Set8bitToRAM(0x5005, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        [Fact]
        public void BIT_fr_Irrr_rrrI() // Ok
        {
            RunTest(
                "BIT F0,(QRS+VWX)",
                c =>
                {
                    c.CPU.REGS.QRS = 0x5000;
                    c.CPU.REGS.VWX = 7;
                    c.MEMC.Set8bitToRAM(0x5007, 0x08);
                    c.CPU.FREGS.SetRegister(0, 12.0f);
                },
                c => Assert.False(c.CPU.FLAGS.IsZero()));
        }

        #endregion

    }
}
