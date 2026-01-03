using System;
using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.Mnemonics;
using Xunit;

namespace ExecutionTests
{
    public class TestEXEC_SUB
    {
        [Fact]
        public void SUB_r_n_decrements_register()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 10;
            cp.Build("SUB A,5\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(5, computer.CPU.REGS.A);
        }

        [Fact]
        public void SUB_r_Innn_nnnI_supports_indexed_absolute()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 50;
            computer.LoadMemAt(9998, new byte[] { 0, 0, 0, 0, 20 });

            cp.Build("SUB A,(10000+2)\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(30, computer.CPU.REGS.A);
        }

        [Fact]
        public void SUB_r_Irrr_rI_uses_pointer_and_index()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 10;
            computer.CPU.REGS.BCD = 30000;
            computer.CPU.REGS.X = 1;
            computer.LoadMemAt(30000, new byte[] { 9, 10, 11 });

            cp.Build("SUB A,(BCD+X)\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
        }

        [Fact]
        public void SUB_rr_nn_subtracts_16bit_immediate()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.AB = 0x1234;
            cp.Build("SUB AB,0x0100\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((ushort)0x1134, computer.CPU.REGS.AB);
        }

        [Fact]
        public void SUB_rr_Irrr_nnnI_reads_indexed_pointer()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.AB = 0x0010;
            computer.CPU.REGS.CDE = 40000;
            computer.CPU.REGS.LM = 4;
            computer.MEMC.Set16bitToRAM(40004, 0x0002);

            cp.Build("SUB AB,(CDE+LM)\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((ushort)0x000E, computer.CPU.REGS.AB);
        }

        [Fact]
        public void SUB_InnnI_nnnn_n_subtracts_block_with_count()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.LoadMemAt(0x5000, new byte[] { 5, 5, 5, 5 });
            cp.Build("SUB (0x5000),0x01020304,3\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0x5000, 4);
            Assert.Equal(new byte[] { 3, 2, 1, 5 }, actual);
        }

        [Fact]
        public void SUB_InnnI_nnnn_n_nnn_repeats_block_subtract()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.LoadMemAt(0x6000, new byte[] { 10, 10, 10, 10 });
            cp.Build("SUB (0x6000),0x00000001,1,4\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0x6000, 4);
            Assert.Equal(new byte[] { 6, 10, 10, 10 }, actual);
        }

        [Fact]
        public void SUB_InnnI_rrr_subtracts_24bit_register_from_memory()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.XYZ = 0x000102;
            computer.MEMC.Set24bitToRAM(0x7000, 0x000200);

            cp.Build("SUB (0x7000),XYZ\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((uint)0x0000FE, computer.MEMC.Get24bitFromRAM(0x7000));
        }

        [Fact]
        public void SUB_Innn_rI_r_writes_to_indexed_memory()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.Z = 2;
            computer.CPU.REGS.B = 3;
            computer.LoadMemAt(0x8000, new byte[] { 5, 6, 7, 8 });

            cp.Build("SUB (0x8000+Z),B\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((byte)4, computer.MEMC.Get8bitFromRAM(0x8002));
        }

        [Fact]
        public void SUB_Irrr_nnnI_rr_subtracts_word_from_pointer_with_offset()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.CDE = 0x9000;
            computer.CPU.REGS.LM = 0x0004;
            computer.CPU.REGS.IJ = 0x0001;
            computer.MEMC.Set16bitToRAM(0x9004, 0x0002);

            cp.Build("SUB (CDE+LM),IJ\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((ushort)0x0001, computer.MEMC.Get16bitFromRAM(0x9004));
        }

        [Fact]
        public void SUB_InnnI_Innn_rrI_n_rrr_uses_value_offset_and_repeat()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.XYZ = 1; // repeat once
            computer.LoadMemAt(0xA000, new byte[] { 10, 10, 10, 10 });
            computer.LoadMemAt(0xA100, new byte[] { 0, 0, 0, 5, 0, 0 });

            cp.Build("SUB (0xA000),(0xA100+1),3,XYZ\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0xA000, 4);
            Assert.Equal(new byte[] { 10, 10, 5, 10 }, actual);
        }

        [Fact]
        public void SUB_r_fr_converts_float_and_applies_sign()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 10;
            computer.CPU.FREGS.SetRegister(0, -3.4f);

            cp.Build("SUB A,FR0\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(13, computer.CPU.REGS.A);
        }

        [Fact]
        public void SUB_fr_r_subtracts_register_value_from_float()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.FREGS.SetRegister(0, 5.5f);
            computer.CPU.REGS.A = 2;

            cp.Build("SUB FR0,A\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(3.5f, computer.CPU.FREGS.GetRegister(0));
        }

        [Fact]
        public void SUB_InnnI_nnn_n_subtracts_single_byte()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.LoadMemAt(0xB000, new byte[] { 8, 1, 1 });
            cp.Build("SUB (0xB000),7,1\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((byte)1, computer.MEMC.Get8bitFromRAM(0xB000));
        }
    }
}
