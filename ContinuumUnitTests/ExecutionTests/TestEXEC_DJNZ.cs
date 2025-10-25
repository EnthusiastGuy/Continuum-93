using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_DJNZ
    {
        [Fact]
        public void TestEXEC_DJNZ_r_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A, 7
                LD B, 10
            .Loop
                INC B
                DJNZ A, .Loop
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_r_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Loop
                LD A, 7
                LD B, 10
            .Loop
                INC B
                DJNZ A, XYZ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD AB, 400
                LD CD, 10
            .Loop
                INC CD
                DJNZ AB, .Loop
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(410, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Loop
                LD AB, 400
                LD CD, 10
            .Loop
                INC CD
                DJNZ AB, XYZ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(410, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD ABC, 400000
                LD DEF, 10000
            .Loop
                INC DEF
                DJNZ ABC, .Loop
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(410000, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Loop
                LD ABC, 400000
                LD DEF, 10000
            .Loop
                INC DEF
                DJNZ ABC, XYZ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(410000, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rrrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD ABCD, 400000
                LD EFGH, 10000
            .Loop
                INC EFGH
                DJNZ ABCD, .Loop
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(410000, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Loop
                LD ABCD, 400000
                LD EFGH, 10000
            .Loop
                INC EFGH
                DJNZ ABCD, XYZ
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(410000, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }



        [Fact]
        public void TestEXEC_DJNZ_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD B, 10
            .Loop
                INC B
                DJNZ (.Data), .Loop

                LD A, (.Data)
                BREAK
            .Data
                #DB 0x07
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Data
                LD A, 7
                LD B, 10
            .Loop
                INC B
                DJNZ (XYZ), .Loop

                LD A, (XYZ)
                BREAK
            .Data
                #DB 0x07
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ16_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD CD, 10
            .Loop
                INC CD
                DJNZ16 (.Data), .Loop

                LD AB, (.Data)
                BREAK
            .Data
                #DB 0x0100
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(266, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ16_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Data
                LD AB, 0x0100
                LD CD, 10
            .Loop
                INC CD
                DJNZ16 (XYZ), .Loop

                LD AB, (.Data)
                BREAK
            .Data
                #DB 0x0100
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(266, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD DEF, 10
            .Loop
                INC DEF
                DJNZ24 (.Data), .Loop

                LD ABC, (.Data)
                BREAK
            .Data
                #DB 0x010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(65803, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Data
                LD ABC, 0x0100
                LD DEF, 10
            .Loop
                INC DEF
                DJNZ24 (XYZ), .Loop

                LD ABC, (.Data)
                BREAK
            .Data
                #DB 0x010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(65803, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ32_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD EFGH, 10
            .Loop
                INC EFGH
                DJNZ32 (.Data), .Loop

                LD ABCD, (.Data)
                BREAK
            .Data
                #DB 0x01010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(16843019, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ32_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, .Data
                LD ABCD, 0x0100
                LD EFGH, 10
            .Loop
                INC EFGH
                DJNZ32 (XYZ), .Loop

                LD ABCD, (.Data)
                BREAK
            .Data
                #DB 0x01010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(16843019, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD B, 10
            .Loop
                INC B
                DJNZ (.Data), KLM

                LD A, (.Data)
                BREAK
            .Data
                #DB 0x07
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD XYZ, .Data
                LD A, 7
                LD B, 10
            .Loop
                INC B
                DJNZ (XYZ), KLM

                LD A, (XYZ)
                BREAK
            .Data
                #DB 0x07
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ16_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD CD, 10
            .Loop
                INC CD
                DJNZ16 (.Data), KLM

                LD AB, (.Data)
                BREAK
            .Data
                #DB 0x0100
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(266, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ16_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD XYZ, .Data
                LD AB, 0x0100
                LD CD, 10
            .Loop
                INC CD
                DJNZ16 (XYZ), KLM

                LD AB, (.Data)
                BREAK
            .Data
                #DB 0x0100
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.AB);
            Assert.Equal(266, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ24_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop   
                LD DEF, 10
            .Loop
                INC DEF
                DJNZ24 (.Data), KLM

                LD ABC, (.Data)
                BREAK
            .Data
                #DB 0x010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(65803, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ24_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD XYZ, .Data
                LD ABC, 0x0100
                LD DEF, 10
            .Loop
                INC DEF
                DJNZ24 (XYZ), KLM

                LD ABC, (.Data)
                BREAK
            .Data
                #DB 0x010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABC);
            Assert.Equal(65803, (long)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ32_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD EFGH, 10
            .Loop
                INC EFGH
                DJNZ32 (.Data), KLM

                LD ABCD, (.Data)
                BREAK
            .Data
                #DB 0x01010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(16843019, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DJNZ32_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, .Loop
                LD XYZ, .Data
                LD ABCD, 0x0100
                LD EFGH, 10
            .Loop
                INC EFGH
                DJNZ32 (XYZ), KLM

                LD ABCD, (.Data)
                BREAK
            .Data
                #DB 0x01010101
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.ABCD);
            Assert.Equal(16843019, (long)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
