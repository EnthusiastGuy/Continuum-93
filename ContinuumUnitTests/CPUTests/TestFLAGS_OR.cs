using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_OR
    {
        [Fact]
        public void TestFLAG_OR_CARRY_RESET()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                ADD A, 0xFF ; Set carry
                OR A, 0xFF
                JP NC, .Ok1
                LD X, 1
                BREAK
            .Ok1
                LD A,0x80
                ADD A, 0xFF
                OR A, A
                JP NC, .Ok2
                LD X, 2
                BREAK
            .Ok2
                LD A,0x80
                ADD A, 0xFF
                OR AB, 0xFFFF
                JP NC, .Ok3
                LD X, 3
                BREAK
            .Ok3
                LD A,0x80
                ADD A, 0xFF
                OR AB, AB
                JP NC, .Ok4
                LD X, 4
                BREAK
            .Ok4
                LD A,0x80
                ADD A, 0xFF
                OR ABC, 0xFFFFFF
                JP NC, .Ok5
                LD X, 5
                BREAK
            .Ok5
                LD A,0x80
                ADD A, 0xFF
                OR ABC, ABC
                JP NC, .Ok6
                LD X, 6
                BREAK
            .Ok6
                LD A,0x80
                ADD A, 0xFF
                OR ABCD, 0xFFFFFFFF
                JP NC, .Ok7
                LD X, 7
                BREAK
            .Ok7
                LD A,0x80
                ADD A, 0xFF
                OR ABCD, ABCD
                JP NC, .Ok8
                LD X, 8
                BREAK
            .Ok8
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                OR (BCD), 0xFF
                JP NC, .Ok9
                LD X, 9
                BREAK
            .Ok9
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                OR16 (BCD), 0xFFFF
                JP NC, .Ok10
                LD X, 10
                BREAK
            .Ok10
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                OR24 (BCD), 0xFFFFFF
                JP NC, .Ok11
                LD X, 11
                BREAK
            .Ok11
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                OR32 (BCD), 0xFFFFFFFF
                JP NC, .Ok12
                LD X, 12
                BREAK
            .Ok12
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD E, 0xFF
                OR (BCD), E
                JP NC, .Ok13
                LD X, 13
                BREAK
            .Ok13
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EF, 0xFFFF
                OR (BCD), EF
                JP NC, .Ok14
                LD X, 14
                BREAK
            .Ok14
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EFG, 0xFFFFFF
                OR (BCD), EFG
                JP NC, .Ok15
                LD X, 15
                BREAK
            .Ok15
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EFGH, 0xFFFFFFFF
                OR (BCD), EFGH
                JP NC, .Ok16
                LD X, 16
                BREAK
            .Ok16
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }

        [Fact]
        public void TestFLAG_OR_ZERO()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                OR A, 0xFF
                JP NZ, .OkNotZero1
                LD X, 1
                BREAK
            .OkNotZero1
                LD A, 0
                OR A, 0
                JP Z, .OkZero1
                LD X, 2
                BREAK
            .OkZero1
                LD A,0x80
                OR A, A
                JP NZ, .OkNotZero2
                LD X, 3
                BREAK
            .OkNotZero2
                LD A, 0
                LD R, 0
                OR A, R
                JP Z, .OkZero2
                LD X, 4
                BREAK
            .OkZero2
                LD A,0x80
                OR AB, 0xFFFF
                JP NZ, .OkNotZero3
                LD X, 5
                BREAK
            .OkNotZero3
                LD AB, 0
                OR AB, 0
                JP Z, .OkZero3
                LD X, 6
                BREAK
            .OkZero3
                LD A,0x80
                OR AB, AB
                JP NZ, .OkNotZero4
                LD X, 7
                BREAK
            .OkNotZero4
                LD CD, 0
                LD AB, 0
                OR AB, CD
                JP Z, .OkZero4
                LD X, 8
            .OkZero4
                LD A,0x80
                OR ABC, 0xFFFFFF
                JP NZ, .OkNotZero5
                LD X, 9
                BREAK
            .OkNotZero5
                LD ABC, 0
                OR ABC, 0
                JP Z, .OkZero5
                LD X, 10
                BREAK
            .OkZero5
                LD A,0x80
                OR ABC, ABC
                JP NZ, .OkNotZero6
                LD X, 11
                BREAK
            .OkNotZero6
                LD ABC, 0
                LD DEF, 0
                OR ABC, DEF
                JP Z, .OkZero6
                LD X, 12
                BREAK
            .OkZero6
                LD A,0x80
                OR ABCD, 0xFFFFFFFF
                JP NZ, .OkNotZero7
                LD X, 13
                BREAK
            .OkNotZero7
                LD ABC, 0
                OR ABC, 0
                JP Z, .OkZero7
                LD X, 14
                BREAK
            .OkZero7
                LD A,0x80
                OR ABCD, ABCD
                JP NZ, .OkNotZero8
                LD X, 15
                BREAK
            .OkNotZero8
                LD ABCD, 0
                LD EFGH, 0
                OR ABCD, EFGH
                JP Z, .OkZero8
                LD X, 16
                BREAK
            .OkZero8
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                OR (BCD), 0xFF
                JP NZ, .OkNotZero9
                LD X, 17
                BREAK
            .OkNotZero9
                LD (BCD), 0
                OR (BCD), 0
                JP Z, .OkZero9
                LD X, 18
                BREAK
            .OkZero9
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                OR16 (BCD), 0xFFFF
                JP NZ, .OkNotZero10
                LD X, 19
                BREAK
            .OkNotZero10
                LD16 (BCD), 0
                OR16 (BCD), 0
                JP Z, .OkZero10
                LD X, 20
                BREAK
            .OkZero10
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                OR24 (BCD), 0xFFFFFF
                JP NZ, .OkNotZero11
                LD X, 21
                BREAK
            .OkNotZero11
                LD24 (BCD), 0
                OR24 (BCD), 0
                JP Z, .OkZero11
                LD X, 22
                BREAK
            .OkZero11
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                OR32 (BCD), 0xFFFFFFFF
                JP NZ, .OkNotZero12
                LD X, 23
                BREAK
            .OkNotZero12
                LD32 (BCD), 0
                OR32 (BCD), 0
                JP Z, .OkZero12
                LD X, 24
                BREAK
            .OkZero12
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD E, 0xFF
                OR (BCD), E
                JP NZ, .OkNotZero13
                LD X, 25
                BREAK
            .OkNotZero13
                LD E, 0
                LD (BCD), 0
                OR (BCD), E
                JP Z, .OkZero13
                LD X, 26
                BREAK
            .OkZero13
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EF, 0xFFFF
                OR (BCD), EF
                JP NZ, .OkNotZero14
                LD X, 27
                BREAK
            .OkNotZero14
                LD EF, 0
                LD16 (BCD), 0
                OR (BCD), EF
                JP Z, .OkZero14
                LD X, 28
                BREAK
            .OkZero14
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EFG, 0xFFFFFF
                OR (BCD), EFG
                JP NZ, .OkNotZero15
                LD X, 29
                BREAK
            .OkNotZero15
                LD EFG, 0
                LD24 (BCD), 0
                OR (BCD), EFG
                JP Z, .OkZero15
                LD X, 30
                BREAK
            .OkZero15
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EFGH, 0xFFFFFFFF
                OR (BCD), EFGH
                JP NZ, .OkNotZero16
                LD X, 31
                BREAK
            .OkNotZero16
                LD EFGH, 0
                LD32 (BCD), 0
                OR (BCD), EFGH
                JP Z, .OkZero16
                LD X, 32
                BREAK
            .OkZero16
                BREAK

            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.X);
        }
    }
}
