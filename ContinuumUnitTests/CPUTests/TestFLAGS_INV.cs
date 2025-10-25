using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_INV
    {
        [Fact]
        public void TestFLAG_INV_CARRY_RESET()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                ADD A, 0xFF ; Set carry
                INV A
                JP NC, .Ok1
                LD X, 1
                BREAK
            .Ok1
                LD A,0x80
                ADD A, 0xFF
                INV AB
                JP NC, .Ok2
                LD X, 2
                BREAK
            .Ok2
                LD A,0x80
                ADD A, 0xFF
                INV ABC
                JP NC, .Ok3
                LD X, 3
                BREAK
            .Ok3
                LD A,0x80
                ADD A, 0xFF
                INV ABCD
                JP NC, .Ok4
                LD X, 4
                BREAK
            .Ok4
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_INV_ZERO()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                INV A
                JP NZ, .OkNotZero1
                LD X, 1
                BREAK
            .OkNotZero1
                LD A, 0xFF
                INV A
                JP Z, .OkZero1
                LD X, 2
                BREAK
            .OkZero1
                LD A,0x80
                INV AB
                JP NZ, .OkNotZero2
                LD X, 3
                BREAK
            .OkNotZero2
                LD AB, 0xFFFF
                INV AB
                JP Z, .OkZero2
                LD X, 4
                BREAK
            .OkZero2
                LD ABC, 0x808080
                INV ABC
                JP NZ, .OkNotZero3
                LD X, 5
                BREAK
            .OkNotZero3
                LD ABC, 0xFFFFFF
                INV ABC
                JP Z, .OkZero3
                LD X, 6
                BREAK
            .OkZero3
                LD ABCD, 0x80808080
                INV ABCD
                JP NZ, .OkNotZero4
                LD X, 7
                BREAK
            .OkNotZero4
                LD ABCD, 0xFFFFFFFF
                INV ABCD
                JP Z, .OkZero4
                LD X, 8
                BREAK
            .OkZero4
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }
    }
}
