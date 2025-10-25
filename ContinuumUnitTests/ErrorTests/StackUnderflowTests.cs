using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ErrorTests
{
    public class StackUnderflowTests
    {
        [Fact]
        public void TestEXEC_POP_r_Underflow() =>
        ExecutePopUnderflowTest(
            spr: 0,
            setupCode: "POP A"
        );

        [Fact]
        public void TestEXEC_POP_rr_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 1,
                setupCode: "POP AB"
            );

        [Fact]
        public void TestEXEC_POP_rrr_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 2,
                setupCode: "POP ABC"
            );

        [Fact]
        public void TestEXEC_POP_rrrr_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 3,
                setupCode: "POP ABCD"
            );

        [Fact]
        public void TestEXEC_POP_r_r_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 2,
                setupCode: "POP A, Z"
            );

        [Fact]
        public void TestEXEC_POP_fr_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 2,
                setupCode: "POP F0"
            );

        [Fact]
        public void TestEXEC_POP_fr_fr_Underflow() =>
            ExecutePopUnderflowTest(
                spr: 12,
                setupCode: "POP F0, F15"
            );

        private static void ExecutePopUnderflowTest(uint spr, string setupCode)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Initialize computer state
            computer.CPU.REGS.SPR = spr;
            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS] = 20;
            computer.CPU.REGS.IPO = 0;

            // Build the program
            cp.Build(@$"
                {setupCode}
                BREAK
                RET
                {InsertLineDuplicate("NOP", 30)}

                LD ABCD, 0x12345678
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            // Run the test
            computer.LoadMem(compiled);
            computer.Run();

            // Assertions
            Assert.Equal(0x12345678, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(SystemMessages.ERR_STACK_UNDERFLOW, computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID]);
            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            TUtils.IncrementCountedTests("errors");
        }

        private static string InsertLineDuplicate(string content, int count)
        {
            if (string.IsNullOrEmpty(content) || count <= 0)
                return string.Empty;

            return string.Join(Environment.NewLine, Enumerable.Repeat(content, count));
        }
    }
}
