using System;
using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Xunit;

namespace ExecutionTests
{
    public class TestEXEC_ADD
    {
        [Fact]
        public void ADD_r_n_increments_8bit_register()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 10;
            cp.Build("ADD A,5\nBREAK");

            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_r_n bytes: " + string.Join(",", bytes));
            Console.WriteLine($"Errors: {cp.Errors} Log: {cp.Log}");
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal(15, computer.CPU.REGS.A);
        }

        [Fact]
        public void ADD_r_Innn_nnnI_supports_indexed_absolute()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 1;
            computer.LoadMemAt(9995, new byte[] { 0, 0, 0, 20, 0, 0, 0, 30 });

            cp.Build("ADD A,(10000+2)\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_r_Innn_nnnI bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal(31, computer.CPU.REGS.A);
        }

        [Fact]
        public void ADD_r_Innn_rI_uses_register_offset()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 5;
            computer.CPU.REGS.Z = 2;
            computer.LoadMemAt(20000, new byte[] { 1, 2, 3, 4, 5 });

            cp.Build("ADD A,(20000+Z)\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(8, computer.CPU.REGS.A);
        }

        [Fact]
        public void ADD_r_Irrr_rI_uses_pointer_and_index()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 1;
            computer.CPU.REGS.BCD = 30000;
            computer.CPU.REGS.X = 1;
            computer.LoadMemAt(30000, new byte[] { 9, 10, 11 });

            cp.Build("ADD A,(BCD+X)\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal(11, computer.CPU.REGS.A);
        }

        [Fact]
        public void ADD_rr_nn_adds_16bit_immediate()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.AB = 0x1000;
            cp.Build("ADD AB,0x1234\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((ushort)0x2234, computer.CPU.REGS.AB);
        }

        [Fact]
        public void ADD_rr_Irrr_nnnI_reads_indexed_pointer()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.AB = 0x0010;
            computer.CPU.REGS.CDE = 40000;
            computer.LoadMemAt(40000, new byte[] { 0x00, 0x00, 0x01, 0x00 });

            cp.Build("ADD AB,(CDE+1)\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_rr_Irrr_nnnI bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal((ushort)0x0011, computer.CPU.REGS.AB);
        }

        [Fact]
        public void ADD_InnnI_nnnn_n_adds_block_with_count()
        {
            Assembler cp = new();
            using var computer = new Computer();

            cp.Build("ADD (0x5000),0x01020304,3\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_InnnI_nnnn_n bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");
            computer.LoadMem(cp.GetCompiledCode());
            computer.LoadMemAt(0x5000, new byte[] { 1, 1, 1, 1 });

            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0x5000, 4);
            Assert.Equal(new byte[] { 3, 4, 5, 1 }, actual);
        }

        [Fact]
        public void ADD_InnnI_nnnn_n_nnn_repeats_block_add()
        {
            Assembler cp = new();
            using var computer = new Computer();

            cp.Build("ADD (0x6000),0x00000001,1,4\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_InnnI_nnnn_n_nnn bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");
            computer.LoadMem(cp.GetCompiledCode());
            computer.LoadMemAt(0x6000, new byte[] { 1, 1, 1, 1 });

            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0x6000, 4);
            Assert.Equal(new byte[] { 2, 2, 2, 2 }, actual);
        }

        [Fact]
        public void ADD_InnnI_rrr_adds_24bit_register_to_memory()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.XYZ = 0x000102;
            computer.MEMC.Set24bitToRAM(0x7000, 0x000100);

            cp.Build("ADD (0x7000),XYZ\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_InnnI_rrr bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal((uint)0x000202, computer.MEMC.Get24bitFromRAM(0x7000));
        }

        [Fact]
        public void ADD_Innn_rI_r_writes_to_indexed_memory()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.Z = 2;
            computer.CPU.REGS.B = 3;
            computer.LoadMemAt(0x8000, new byte[] { 1, 1, 1, 1 });

            cp.Build("ADD (0x8000+Z),B\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((byte)4, computer.MEMC.Get8bitFromRAM(0x8002));
        }

        [Fact]
        public void ADD_Irrr_nnnI_rr_adds_word_to_pointer_with_offset()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.CDE = 0x9000;
            computer.CPU.REGS.LM = 0x0004;
            computer.CPU.REGS.IJ = 0x0001;
            computer.MEMC.Set16bitToRAM(0x9004, 0x0002);

            cp.Build("ADD (CDE+LM),IJ\nBREAK");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((ushort)0x0003, computer.MEMC.Get16bitFromRAM(0x9004));
        }

        [Fact]
        public void ADD_InnnI_Innn_rrI_n_rrr_uses_value_offset_and_repeat()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.XYZ = 2; // repeat twice
            computer.LoadMemAt(0xA000, new byte[] { 1, 1, 1, 1, 1, 1 });
            computer.LoadMemAt(0xA100, new byte[] { 0, 0, 0, 5, 0, 0, 0, 6 });

            cp.Build("ADD (0xA000),(0xA100+1),3,XYZ\nBREAK");
            Console.WriteLine("ADD_InnnI_Innn_rrI_n_rrr bytes: " + string.Join(",", cp.GetCompiledCode()));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            byte[] actual = computer.MEMC.RAM.GetMemoryAt(0xA000, 6);
            Assert.Equal(new byte[] { 1, 6, 1, 1, 6, 1 }, actual);
        }

        [Fact]
        public void ADD_r_fr_converts_float_and_applies_sign()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.REGS.A = 10;
            computer.CPU.FREGS.SetRegister(0, -3.4f);

            cp.Build("ADD A,FR0\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_r_fr bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal(7, computer.CPU.REGS.A);
        }

        [Fact]
        public void ADD_fr_r_accumulates_into_float_register()
        {
            Assembler cp = new();
            using var computer = new Computer();

            computer.CPU.FREGS.SetRegister(0, 1.5f);
            computer.CPU.REGS.A = 2;

            cp.Build("ADD FR0,A\nBREAK");
            var bytes = cp.GetCompiledCode();
            Console.WriteLine("ADD_fr_r bytes: " + string.Join(",", bytes));
            Console.WriteLine($"GeneralForm: {cp.GetCompiledLine(0)?.GeneralForm}");

            computer.LoadMem(bytes);
            computer.Run();

            Assert.Equal(3.5f, computer.CPU.FREGS.GetRegister(0));
        }
    }
}
