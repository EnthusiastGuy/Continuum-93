using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumUnitTests.ExecutionTests.Repetition_tests
{
    public class TestEXEC_ADD_repeat
    {
        public static IEnumerable<object[]> RepeatCases()
        {
            // count, repeat, first bytes, second bytes, expected bytes (first four)
            yield return new object[] { 1, 1u, new byte[] { 100, 0, 0, 0 }, new byte[] { 25, 0, 0, 0 }, new byte[] { 125, 0, 0, 0 } };
            yield return new object[] { 1, 2u, new byte[] { 100, 0, 0, 0 }, new byte[] { 25, 0, 0, 0 }, new byte[] { 150, 0, 0, 0 } };

            // carry across two bytes: process from higher offset down to lower offset
            yield return new object[] { 2, 1u, new byte[] { 250, 100, 0, 0 }, new byte[] { 60, 25, 0, 0 }, new byte[] { 54, 125, 0, 0 } };
            yield return new object[] { 2, 2u, new byte[] { 250, 100, 0, 0 }, new byte[] { 60, 25, 0, 0 }, new byte[] { 114, 150, 0, 0 } };

            // count=3 to ensure additional bytes remain stable and carry chains correctly over two bytes only here
            yield return new object[] { 3, 2u, new byte[] { 250, 100, 5, 0 }, new byte[] { 60, 25, 1, 0 }, new byte[] { 114, 150, 7, 0 } };

            // explicit big-endian style example: {100,60} + {100,200} with count=2
            yield return new object[] { 2, 1u, new byte[] { 100, 60, 0, 0 }, new byte[] { 100, 200, 0, 0 }, new byte[] { 201, 4, 0, 0 } };

            // count=5, single repeat with a carry into the first byte
            yield return new object[] { 5, 1u, new byte[] { 100, 50, 0, 0, 0 }, new byte[] { 200, 210, 255, 255, 255 }, new byte[] { 45, 4, 255, 255, 255 } };

            // count=10, repeat=3 to cover longer spans and multiple passes; carry only on first repeat
            yield return new object[] {
                10,
                3u,
                new byte[] { 250, 250, 250, 250, 250, 250, 250, 250, 250, 250 },
                new byte[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                new byte[] { 25, 25, 25, 25, 25, 25, 25, 25, 25, 24 }
            };
        }

        [Theory]
        [MemberData(nameof(RepeatCases))]
        public void Test_ADD_repeated_InnnI_InnnI_n_rrr(byte count, uint repeatRegValue, byte[] firstBytes, byte[] secondBytes, byte[] expectedBytes)
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build($@"
                LD ABC, .FirstValue
                LD DEF, .SecondValue
                LD GHI, {repeatRegValue}
                ADD (.FirstValue),(.SecondValue), {count}, GHI
                BREAK

            .FirstValue
                #DB {string.Join(", ", firstBytes)}
            .SecondValue
                #DB {string.Join(", ", secondBytes)}
            ");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            uint firstValueAddress = computer.CPU.REGS.ABC;
            uint secondValueAddress = computer.CPU.REGS.DEF;
            Span<byte> actual = stackalloc byte[firstBytes.Length];
            for (int i = 0; i < firstBytes.Length; i++)
                actual[i] = computer.MEMC.Get8bitFromRAM(firstValueAddress + (uint)i);

            Assert.Equal(expectedBytes, actual.ToArray());

            // ensure source unchanged
            for (int i = 0; i < secondBytes.Length; i++)
                Assert.Equal(secondBytes[i], computer.MEMC.Get8bitFromRAM(secondValueAddress + (uint)i));
        }
    }
}
