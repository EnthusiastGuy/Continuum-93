using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_VCL
    {
        private readonly Assembler assembler = new();
        private byte inputValue;
        private readonly string commonTestSetup =
            @"LD A, 0x33
              LD B, 0b00000000
              INT 0x01, A
              LD (0xFE05C0), 0xFFFFFFFF, 4";

        public static TheoryData<TestInputData> TestData =>
            new()
            {
                new TestInputData { InputValue = 0b00000001, ExpectedResult = true },
                new TestInputData { InputValue = 0b10000000, ExpectedResult = false },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestEXEC_VCL_n(TestInputData inputData)
        {
            using Computer computer = new();
            inputValue = inputData.InputValue;

            assembler.Build($@"
                {commonTestSetup}
                VCL {inputValue}
                BREAK
            ");

            ExecuteAndAssert(inputData.ExpectedResult, computer);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestEXEC_VCL_r(TestInputData inputData)
        {
            using Computer computer = new();
            inputValue = inputData.InputValue;

            assembler.Build($@"
                {commonTestSetup}
                LD X, {inputValue}
                VCL X
                BREAK
            ");

            ExecuteAndAssert(inputData.ExpectedResult, computer);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestEXEC_VCL_InnnI(TestInputData inputData)
        {
            using Computer computer = new();
            inputValue = inputData.InputValue;

            computer.MEMC.Set8bitToRAM(0x2000, inputValue);

            assembler.Build($@"
                {commonTestSetup}
                VCL (0x2000)
                BREAK
            ");

            ExecuteAndAssert(inputData.ExpectedResult, computer);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestEXEC_VCL_IrrrI(TestInputData inputData)
        {
            using Computer computer = new();
            inputValue = inputData.InputValue;

            computer.MEMC.Set8bitToRAM(0x2000, inputValue);

            assembler.Build($@"
                {commonTestSetup}
                LD ABC, 0x2000
                VCL (ABC)
                BREAK
            ");

            ExecuteAndAssert(inputData.ExpectedResult, computer);
        }

        private void ExecuteAndAssert(bool expectedResult, Computer computer)
        {
            byte[] compiled = assembler.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            uint afterVideoData = CalculateBufferSUM(computer.GRAPHICS.GetVideoBuffer());

            if (expectedResult)
            {
                Assert.Equal((uint)0x3FC, afterVideoData);
            }
            else
            {
                Assert.NotEqual((uint)0x3FC, afterVideoData);
            }

            TUtils.IncrementCountedTests("exec");
        }

        // Support
        private static uint CalculateBufferSUM(byte[] input)
        {
            uint sum = 0;

            foreach (byte b in input)
                sum += b;

            return sum;
        }
    }
}
