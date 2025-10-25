using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Continuum93.Emulator.RAM;

namespace ErrorTests
{
    public class StackOverflowTests
    {
        [Fact]
        public void TestEXEC_PUSH_r_Overflow() =>
        ExecutePushOverflowTest(
            spr: 4 * 1024 * 1024 - 1,
            setupCode: "LD A, 0x44\nPUSH A"
        );

        [Fact]
        public void TestEXEC_PUSH_rr_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 2,
                setupCode: "LD AB, 0x4444\nPUSH AB"
            );

        [Fact]
        public void TestEXEC_PUSH_rrr_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 3,
                setupCode: "LD ABC, 0x444444\nPUSH ABC"
            );

        [Fact]
        public void TestEXEC_PUSH_rrrr_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 4,
                setupCode: "LD ABCD, 0x44444444\nPUSH ABCD"
            );

        [Fact]
        public void TestEXEC_PUSH_r_r_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 13,
                setupCode: "PUSH A, Z"
            );

        [Fact]
        public void TestEXEC_PUSH_fr_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 4,
                setupCode: "LD F0, 1200.0\nPUSH F0"
            );

        [Fact]
        public void TestEXEC_PUSH_fr_fr_Overflow() =>
            ExecutePushOverflowTest(
                spr: 4 * 1024 * 1024 - 4*8,
                setupCode: "PUSH F0, F15"
            );

        private static void ExecutePushOverflowTest(uint spr, string setupCode)
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
            Assert.Equal(SystemMessages.ERR_STACK_OVERFLOW, computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID]);
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
