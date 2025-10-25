using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Tools;


namespace CompilerDirectivesTests
{

    public class TestCD_DB
    {
        [Fact]
        public void TestDB_NullTermination()
        {
            Assert.Equal([84, 101, 120, 116, 0], GetBytesFromSource("#DB \"Text\", 0"));
            Assert.Equal([84, 101, 120, 116, 0], GetBytesFromSource("#DB \"Text\0\""));
        }

        [Fact]
        public void TestDB_Semicolons_In_Strings()
        {
            Assert.Equal([84, 101, 120, 116, 59, 0], GetBytesFromSource("#DB \"Text;\", 0"));
        }

        [Fact]
        public void TestDB_Comma_In_DedicatedStrings()
        {
            Assert.Equal([84, 101, 120, 116, 44, 32, 84, 101, 120, 116, 0], GetBytesFromSource("#DB \"Text, Text\", 0"));
        }

        [Fact]
        public void TestDB_Comma_In_SharedStrings()
        {
            Assert.Equal([12, 84, 101, 120, 116, 44, 32, 84, 101, 120, 116, 0], GetBytesFromSource("#DB 12, \"Text, Text\", 0"));
        }

        [Fact]
        public void TestDB_Numbers_Labels()
        {
            Assembler cp = new();

            cp.Build(@"
                    #ORG 100
                .LabelAt100
                    LD A, 12
                    LD BC, 0x0203
                    LD A, C
                    RET
                
                    #ORG 1000
                .LabelDB
                    #DB 0xFF, 0xFF, .LabelAt100, 0xFF
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            Assert.Equal([1, 0, 0, 12, 1, 13, 1, 2, 3, 1, 1, 0, 2, 255], cBlocks[0].Data);
            Assert.Equal([255, 255, 0, 0, 100, 255], cBlocks[1].Data);
            Assert.Equal(100, (int)cBlocks[0].Start);
            Assert.Equal(1000, (int)cBlocks[1].Start);
            Assert.False(cp.BlockManager.HasCollisions());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestDB_Numbers_Labels2()
        {
            Assembler cp = new();

            cp.Build(@"
                    #ORG 100
                .LabelAt100
                    LD A, 12
                    LD BC, 0x0203
                    LD A, C
                    RET
                
                    #ORG 1000
                .LabelDB
                    #DB 0xFF, 0xFF, .LabelAt100, 0xFF, .LabelDB, 0xFF, .EndLabel
                    #DB " + "\"Hello world\"" + @", 0
                .EndLabel
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            Assert.Equal([1, 0, 0, 12, 1, 13, 1, 2, 3, 1, 1, 0, 2, 255], cBlocks[0].Data);
            Assert.Equal([ 255, 255,
                0, 0, 100,  // LabelAt100
                255,
                0, 3, 232,  // LabelDB
                255,
                0, 4, 1,
                72, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100, 0  // Hello world
            ], cBlocks[1].Data);
            Assert.Equal(100, (int)cBlocks[0].Start);
            Assert.Equal(1000, (int)cBlocks[1].Start);
            Assert.False(cp.BlockManager.HasCollisions());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestDB_Numbers_SpecialFormatsHex1()
        {
            Assert.Equal([0], GetBytesFromSource(@"#DB 0x0"));
            Assert.Equal([0], GetBytesFromSource(@"#DB 0x00"));
            Assert.Equal([0, 0], GetBytesFromSource(@"#DB 0x000"));
            Assert.Equal([0, 0], GetBytesFromSource(@"#DB 0x0000"));
            Assert.Equal([0, 0, 0], GetBytesFromSource(@"#DB 0x00000"));
            Assert.Equal([0, 0, 0], GetBytesFromSource(@"#DB 0x000000"));
            Assert.Equal([0, 0, 0, 0], GetBytesFromSource(@"#DB 0x0000000"));
            Assert.Equal([0, 0, 0, 0], GetBytesFromSource(@"#DB 0x00000000"));
            Assert.Equal([], GetBytesFromSource(@"#DB 0x000000000")); // Error
        }

        [Fact]
        public void TestDB_Numbers_SpecialFormatsHex2()
        {
            Assert.Equal([10], GetBytesFromSource(@"#DB 0xA"));
            Assert.Equal([10], GetBytesFromSource(@"#DB 0x0A"));
            Assert.Equal([0, 10], GetBytesFromSource(@"#DB 0x00A"));
            Assert.Equal([0, 10], GetBytesFromSource(@"#DB 0x000A"));
            Assert.Equal([0, 0, 10], GetBytesFromSource(@"#DB 0x0000A"));
            Assert.Equal([0, 0, 10], GetBytesFromSource(@"#DB 0x00000A"));
            Assert.Equal([0, 0, 0, 10], GetBytesFromSource(@"#DB 0x000000A"));
            Assert.Equal([0, 0, 0, 10], GetBytesFromSource(@"#DB 0x0000000A"));
            Assert.Equal([], GetBytesFromSource(@"#DB 0x00000000A")); // Error
        }

        [Fact]
        public void TestDB_Numbers_SpecialFormatsBin1()
        {
            Assert.Equal([10], GetBytesFromSource(@"#DB 0b1010"));
            Assert.Equal([2, 170], GetBytesFromSource(@"#DB 0b1010101010"));
            Assert.Equal([170, 170, 170], GetBytesFromSource(@"#DB 0b10101010_10101010_10101010"));
            Assert.Equal([170, 170, 170, 170], GetBytesFromSource(@"#DB 0b10101010_10101010_10101010_10101010"));

            Assert.Equal([], GetBytesFromSource(@"#DB 0b10101010_10101010_10101010_10101010_10101010"));
        }

        [Fact]
        public void TestDB_Numbers_SpecialFormatsDecimal()
        {
            Assert.Equal([0], GetBytesFromSource(@"#DB 0"));

            Assert.Equal([1], GetBytesFromSource(@"#DB 01"));
            Assert.Equal([0, 1], GetBytesFromSource(@"#DB 001"));
            Assert.Equal([0, 0, 1], GetBytesFromSource(@"#DB 0001"));
            Assert.Equal([0, 0, 0, 1], GetBytesFromSource(@"#DB 00001"));

            Assert.Equal([11], GetBytesFromSource(@"#DB 011"));
            Assert.Equal([0, 11], GetBytesFromSource(@"#DB 0011"));
            Assert.Equal([0, 0, 11], GetBytesFromSource(@"#DB 00011"));
            Assert.Equal([0, 0, 0, 11], GetBytesFromSource(@"#DB 000011"));

            Assert.Equal([255], GetBytesFromSource(@"#DB 0255"));
            Assert.Equal([0, 255], GetBytesFromSource(@"#DB 00255"));
            Assert.Equal([0, 0, 255], GetBytesFromSource(@"#DB 000255"));
            Assert.Equal([0, 0, 0, 255], GetBytesFromSource(@"#DB 0000255"));

            Assert.Equal([], GetBytesFromSource(@"#DB 00000255"));
        }

        [Fact]
        public void TestDB_Numbers_SpecialFormatsOctal()
        {
            Assert.Equal([8], GetBytesFromSource(@"#DB 0o10"));
            Assert.Equal([8], GetBytesFromSource(@"#DB 0o010"));
            Assert.Equal([0, 8], GetBytesFromSource(@"#DB 0o0010"));
            Assert.Equal([0, 0, 8], GetBytesFromSource(@"#DB 0o00010"));
            Assert.Equal([0, 0, 0, 8], GetBytesFromSource(@"#DB 0o000010"));

            Assert.Equal(Array.Empty<byte>(), GetBytesFromSource(@"#DB 0o000001"));
        }

        [Fact]
        public void TestDB_Numbers_NegativeByte()
        {
            Assert.Equal([255], GetBytesFromSource(@"#DB -1"));
            Assert.Equal([255], GetBytesFromSource(@"#DB -01"));
            Assert.Equal([255, 255], GetBytesFromSource(@"#DB -001"));
            Assert.Equal([255, 255, 255], GetBytesFromSource(@"#DB -0001"));
            Assert.Equal([255, 255, 255, 255], GetBytesFromSource(@"#DB -00001"));
            Assert.Equal([], GetBytesFromSource(@"#DB -000001"));
            Assert.Equal([], GetBytesFromSource(@"#DB -0000001"));
        }

        [Fact]
        public void TestDB_Numbers_NegativeByte_HEX()
        {
            Assert.Equal([255], GetBytesFromSource(@"#DB -0x01"));
            Assert.Equal([255, 255], GetBytesFromSource(@"#DB -0x001"));
            Assert.Equal([255, 255], GetBytesFromSource(@"#DB -0x0001"));
            Assert.Equal([255, 255, 255], GetBytesFromSource(@"#DB -0x00001"));
            Assert.Equal([255, 255, 255], GetBytesFromSource(@"#DB -0x000001"));
            Assert.Equal([255, 255, 255, 255], GetBytesFromSource(@"#DB -0x0000001"));
            Assert.Equal([255, 255, 255, 255], GetBytesFromSource(@"#DB -0x00000001"));
            Assert.Equal([], GetBytesFromSource(@"#DB -0x000000001"));
            Assert.Equal([], GetBytesFromSource(@"#DB -0x0000000001"));
        }

        [Fact]
        public void TestDB_Numbers_NegativeByte_BINARY()
        {
            Assert.Equal([255], GetBytesFromSource(@"#DB -0b00000001"));
            Assert.Equal([255, 255], GetBytesFromSource(@"#DB -0b0000000_00000001"));
            Assert.Equal([255, 255, 255], GetBytesFromSource(@"#DB -0b00000000_00000000_00000001"));
            Assert.Equal([255, 255, 255, 255], GetBytesFromSource(@"#DB -0b00000000_00000000_00000000_00000001"));
        }

        [Fact]
        public void TestDB_Numbers_Floating()
        {
            Assert.Equal(
                DataConverter.MergeByteArrays(FloatPointUtils.FloatToBytes(1.3f), FloatPointUtils.FloatToBytes(7.6f)),
                GetBytesFromSource(@"#DB 1.3, 7.6"));
            Assert.NotEqual(
                DataConverter.MergeByteArrays(FloatPointUtils.FloatToBytes(1.25f), FloatPointUtils.FloatToBytes(2.26f)),
                GetBytesFromSource(@"#DB 1.25f, 2.26f"));
            Assert.Equal(
                DataConverter.MergeByteArrays(FloatPointUtils.FloatToBytes(-800.62f), FloatPointUtils.FloatToBytes(-0.006f)),
                GetBytesFromSource(@"#DB -800.62, -0.006"));
        }

        [Fact]
        public void TestDB_Repetition_Directive_Simple()
        {
            Assembler cp = new();

            cp.Build(@"
                    #ORG 100
                    #DB [1] 1, [2]2, [3]3, [4]4, 5
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(1, cp.BlockManager.BlocksCount());
            Assert.Equal([1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5], cBlocks[0].Data);
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestDB_Repetition_Directive_Complex()
        {
            Assembler cp = new();

            cp.Build(@"
                    #ORG 100
                    #DB 0x0A, [10] 0x0B, 0xC
                    #DB ""Test"", 0, [2] 255
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(1, cp.BlockManager.BlocksCount());
            Assert.Equal([10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 12, 84, 101, 115, 116, 0, 255, 255], cBlocks[0].Data);
            TUtils.IncrementCountedTests("directives");
        }

        private static byte[] GetBytesFromSource(string source)
        {
            Assembler cp = new();

            cp.Build("#ORG 100" + Environment.NewLine + source);

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            return cBlocks[0].Data;
        }
    }
}
