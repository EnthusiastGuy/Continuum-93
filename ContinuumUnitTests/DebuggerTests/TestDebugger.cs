using Continuum93.CodeAnalysis;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace DebuggerTests
{
    [Collection("Debugger tests")]
    public class TestDebugger
    {
        [Fact]
        public void TestDebuggerDissassemblySimple1()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A, 0x10
                LD B, 0x20
                LD F0, F1
            .Repeat
                DEC AB
                XOR A, B
                CP AB, 0
                JP NZ, .Repeat
                LD B, A
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(0, compiled);

            ContinuumDebugger.DebugInstructionsCount = 12;
            ContinuumDebugger.RunAt(0, computer);
            string dissassembly = ContinuumDebugger.GetDissassembled();
            //string dissassemblyAddress = ContinuumDebugger.GetDissassembledWithAddresses();
            string dissassemblyFull = ContinuumDebugger.GetDissassembledFull();

            Assert.Equal(12, ContinuumDebugger.Instructions.Count);
            Assert.Equal(TUtils.GetFormattedAsm(
                "LD A, 0x10",
                "LD B, 0x20",
                "LD F0, F1",
                "DEC AB",
                "XOR A, B",
                "CP AB, 0x0000",
                "JP NZ, 0x00000C",
                "LD B, A",
                "RET",
                "NOP",
                "NOP",
                "NOP"
            ), dissassembly);

            Assert.Equal(TUtils.GetFormattedAsm(
                "000000|01 00 00 10|LD A, 0x10",
                "000004|01 00 01 20|LD B, 0x20",
                "000008|01 F3 00 01|LD F0, F1",
                "00000C|14 01 00|DEC AB",
                "00000F|0F 04 01|XOR A, B",
                "000012|12 08 00 00 00|CP AB, 0x0000",
                "000017|17 20 00 00 0C|JP NZ, 0x00000C",
                "00001C|01 01 01 00|LD B, A",
                "000020|FF|RET",
                "000021|00|NOP",
                "000022|00|NOP",
                "000023|00|NOP"
            ), dissassemblyFull);//*/

            TUtils.IncrementCountedTests("debugger");
        }

        [Fact]
        public void TestDebuggerDissassemblySimple2()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A, -100
                SETF NZ
                RESF C
                INVF GTE
                LD F0, F1
                LD F2, AB
                ADD F0, F2
                SUB F1, AB
                DIV F2, F0
                MUL F1, ABCD
                EX F1, F0
                CP F1, -3.5
                LDF A
                LDF A, 0b00000001
                SIN F0, F1
                COS F0, F0
                POW F0, 2.0
                POW F0, F1
                SQR F1, F0
                CBR F1
                ISQR F3, F1
                ISGN F3
                ABS F4, F3
                FLOOR F4
                CEIL F3
                SDIV ABCD, -10
                SCP ABCD, -1

                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(0, compiled);

            ContinuumDebugger.DebugInstructionsCount = 30;
            ContinuumDebugger.RunAt(0, computer);
            string dissassembly = ContinuumDebugger.GetDissassembled();
            //string dissassemblyAddress = ContinuumDebugger.GetDissassembledWithAddresses();
            string dissassemblyFull = ContinuumDebugger.GetDissassembledFull();

            Assert.Equal(30, ContinuumDebugger.Instructions.Count);
            Assert.Equal(TUtils.GetFormattedAsm(
                "LD A, 0x9C",
                "SETF NZ",
                "RESF C",
                "INVF GTE",
                "LD F0, F1",
                "LD F2, AB",
                "ADD F0, F2",
                "SUB F1, AB",
                "DIV F2, F0",
                "MUL F1, ABCD",
                "EX F1, F0",
                "CP F1, -3.5",
                "LDF A",
                "LDF A, 0x01",
                "SIN F0, F1",
                "COS F0, F0",
                "POW F0, 2",
                "POW F0, F1",
                "SQR F1, F0",
                "CBR F1",
                "ISQR F3, F1",
                "ISGN F3",
                "ABS F4, F3",
                "FLOOR F4",
                "CEIL F3",
                "SDIV ABCD, -0x000A",
                "SCP ABCD, -0x00000001",
                "RET",
                "NOP",
                "NOP"
            ), dissassembly);

            Assert.Equal(TUtils.GetFormattedAsm(
                "000000|01 00 00 9C|LD A, 0x9C",
                "000004|24 00|SETF NZ",
                "000006|25 09|RESF C",
                "000008|26 07|INVF GTE",
                "00000A|01 F3 00 01|LD F0, F1",
                "00000E|01 E6 02 00|LD F2, AB",
                "000012|02 F3 00 02|ADD F0, F2",
                "000016|03 E6 01 00|SUB F1, AB",
                "00001A|04 F3 02 00|DIV F2, F0",
                "00001E|05 E8 01 00|MUL F1, ABCD",
                "000022|11 10 10|EX F1, F0",
                "000025|12 3C 01 C0 60 00 00|CP F1, -3.5",
                "00002C|27 00|LDF A",
                "00002E|27 60 01|LDF A, 0x01",
                "000031|66 10 01|SIN F0, F1",
                "000034|67 10 00|COS F0, F0",
                "000037|60 2C 00 40 00 00 00|POW F0, 2",
                "00003E|60 00 01|POW F0, F1",
                "000041|61 1C 10|SQR F1, F0",
                "000044|62 00 01|CBR F1",
                "000047|63 1C 31|ISQR F3, F1",
                "00004A|64 03|ISGN F3",
                "00004C|80 01 04 03|ABS F4, F3",
                "000050|82 00 04|FLOOR F4",
                "000053|83 00 03|CEIL F3",
                "000056|5C 24 1F F6|SDIV ABCD, -0x000A",
                "00005A|5E 18 00 FF FF FF FF|SCP ABCD, -0x00000001",
                "000061|FF|RET",
                "000062|00|NOP",
                "000063|00|NOP"
            ), dissassemblyFull);

            TUtils.IncrementCountedTests("debugger");
        }
    }
}
