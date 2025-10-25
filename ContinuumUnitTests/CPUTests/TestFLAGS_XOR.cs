using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_XOR
    {
        [Fact]
        public void TestFLAG_XOR_CARRY_RESET()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                ADD A, 0xFF ; Set carry
                XOR A, 0xFF
                JP NC, .Ok1
                LD X, 1
                BREAK
            .Ok1
                LD A,0x80
                ADD A, 0xFF
                XOR A, A
                JP NC, .Ok2
                LD X, 2
                BREAK
            .Ok2
                LD A,0x80
                ADD A, 0xFF
                XOR AB, 0xFFFF
                JP NC, .Ok3
                LD X, 3
                BREAK
            .Ok3
                LD A,0x80
                ADD A, 0xFF
                XOR AB, AB
                JP NC, .Ok4
                LD X, 4
                BREAK
            .Ok4
                LD A,0x80
                ADD A, 0xFF
                XOR ABC, 0xFFFFFF
                JP NC, .Ok5
                LD X, 5
                BREAK
            .Ok5
                LD A,0x80
                ADD A, 0xFF
                XOR ABC, ABC
                JP NC, .Ok6
                LD X, 6
                BREAK
            .Ok6
                LD A,0x80
                ADD A, 0xFF
                XOR ABCD, 0xFFFFFFFF
                JP NC, .Ok7
                LD X, 7
                BREAK
            .Ok7
                LD A,0x80
                ADD A, 0xFF
                XOR ABCD, ABCD
                JP NC, .Ok8
                LD X, 8
                BREAK
            .Ok8
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                XOR (BCD), 0xFF
                JP NC, .Ok9
                LD X, 9
                BREAK
            .Ok9
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                XOR16 (BCD), 0xFFFF
                JP NC, .Ok10
                LD X, 10
                BREAK
            .Ok10
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                XOR24 (BCD), 0xFFFFFF
                JP NC, .Ok11
                LD X, 11
                BREAK
            .Ok11
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                XOR32 (BCD), 0xFFFFFFFF
                JP NC, .Ok12
                LD X, 12
                BREAK
            .Ok12
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD E, 0xFF
                XOR (BCD), E
                JP NC, .Ok13
                LD X, 13
                BREAK
            .Ok13
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EF, 0xFFFF
                XOR (BCD), EF
                JP NC, .Ok14
                LD X, 14
                BREAK
            .Ok14
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EFG, 0xFFFFFF
                XOR (BCD), EFG
                JP NC, .Ok15
                LD X, 15
                BREAK
            .Ok15
                LD A,0x80
                ADD A, 0xFF
                LD BCD, 20000
                LD EFGH, 0xFFFFFFFF
                XOR (BCD), EFGH
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
        public void TestFLAG_XOR_ZERO()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A,0x80
                XOR A, 0xFF
                JP NZ, .OkNotZero1
                LD X, 1
                BREAK
            .OkNotZero1
                LD A, 0
                XOR A, 0
                JP Z, .OkZero1
                LD X, 2
                BREAK
            .OkZero1
                LD A, 0x80
                LD B, 0xFF
                XOR A, B
                JP NZ, .OkNotZero2
                LD X, 3
                BREAK
            .OkNotZero2
                LD A, 0xAA
                LD R, 0xAA
                XOR A, R
                JP Z, .OkZero2
                LD X, 4
                BREAK
            .OkZero2
                LD A,0x80
                XOR AB, 0xFFFF
                JP NZ, .OkNotZero3
                LD X, 5
                BREAK
            .OkNotZero3
                LD AB, 0xAA
                XOR AB, 0xAA
                JP Z, .OkZero3
                LD X, 6
                BREAK
            .OkZero3
                LD AB, 0x8000
                LD CD, 0xFFFF
                XOR AB, CD
                JP NZ, .OkNotZero4
                LD X, 7
                BREAK
            .OkNotZero4
                LD CD, 0xAB
                LD AB, 0xAB
                XOR AB, CD
                JP Z, .OkZero4
                LD X, 8
            .OkZero4
                LD A,0x80
                XOR ABC, 0xFFFFFF
                JP NZ, .OkNotZero5
                LD X, 9
                BREAK
            .OkNotZero5
                LD ABC, 0xABCDEF
                XOR ABC, 0xABCDEF
                JP Z, .OkZero5
                LD X, 10
                BREAK
            .OkZero5
                LD ABC, 0xABCDEF
                LD DEF, 0x123456
                XOR ABC, DEF
                JP NZ, .OkNotZero6
                LD X, 11
                BREAK
            .OkNotZero6
                LD ABC, 0xABCDEF
                LD DEF, 0xABCDEF
                XOR ABC, DEF
                JP Z, .OkZero6
                LD X, 12
                BREAK
            .OkZero6
                LD ABCD,0xABCDEF12
                XOR ABCD, 0xFFFFFFFF
                JP NZ, .OkNotZero7
                LD X, 13
                BREAK
            .OkNotZero7
                LD ABC, 0xABCDEF
                XOR ABC, 0xABCDEF
                JP Z, .OkZero7
                LD X, 14
                BREAK
            .OkZero7
                LD ABCD, 0x8090A0B0
                LD EFGH, 0x12345678
                XOR ABCD, EFGH
                JP NZ, .OkNotZero8
                LD X, 15
                BREAK
            .OkNotZero8
                LD ABCD, 0x8090A0B0
                LD EFGH, 0x8090A0B0
                XOR ABCD, EFGH
                JP Z, .OkZero8
                LD X, 16
                BREAK
            .OkZero8
                LD A, 0x80
                LD BCD, 20000
                LD (BCD), 0x80
                XOR (BCD), 0xFF
                JP NZ, .OkNotZero9
                LD X, 17
                BREAK
            .OkNotZero9
                LD (BCD), 0x80
                XOR (BCD), 0x80
                JP Z, .OkZero9
                LD X, 18
                BREAK
            .OkZero9
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                XOR16 (BCD), 0xFFFF
                JP NZ, .OkNotZero10
                LD X, 19
                BREAK
            .OkNotZero10
                LD16 (BCD), 0x2030
                XOR16 (BCD), 0x2030
                JP Z, .OkZero10
                LD X, 20
                BREAK
            .OkZero10
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                XOR24 (BCD), 0xFFFFFF
                JP NZ, .OkNotZero11
                LD X, 21
                BREAK
            .OkNotZero11
                LD24 (BCD), 0x203040
                XOR24 (BCD), 0x203040
                JP Z, .OkZero11
                LD X, 22
                BREAK
            .OkZero11
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                XOR32 (BCD), 0xFFFFFFFF
                JP NZ, .OkNotZero12
                LD X, 23
                BREAK
            .OkNotZero12
                LD32 (BCD), 0x12345678
                XOR32 (BCD), 0x12345678
                JP Z, .OkZero12
                LD X, 24
                BREAK
            .OkZero12
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD E, 0xFF
                XOR (BCD), E
                JP NZ, .OkNotZero13
                LD X, 25
                BREAK
            .OkNotZero13
                LD E, 0x80
                LD (BCD), 0x80
                XOR (BCD), E
                JP Z, .OkZero13
                LD X, 26
                BREAK
            .OkZero13
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EF, 0xFFFF
                XOR (BCD), EF
                JP NZ, .OkNotZero14
                LD X, 27
                BREAK
            .OkNotZero14
                LD EF, 0x8090
                LD16 (BCD), 0x8090
                XOR (BCD), EF
                JP Z, .OkZero14
                LD X, 28
                BREAK
            .OkZero14
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EFG, 0xFFFFFF
                XOR (BCD), EFG
                JP NZ, .OkNotZero15
                LD X, 29
                BREAK
            .OkNotZero15
                LD EFG, 0x8090A0
                LD24 (BCD), 0x8090A0
                XOR (BCD), EFG
                JP Z, .OkZero15
                LD X, 30
                BREAK
            .OkZero15
                LD A,0x80
                LD BCD, 20000
                LD (BCD), 0x80
                LD EFGH, 0xFFFFFFFF
                XOR (BCD), EFGH
                JP NZ, .OkNotZero16
                LD X, 31
                BREAK
            .OkNotZero16
                LD EFGH, 0x8090A0B0
                LD32 (BCD), 0x8090A0B0
                XOR (BCD), EFGH
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
