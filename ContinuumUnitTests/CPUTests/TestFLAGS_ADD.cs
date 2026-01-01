using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using System;
using System.Collections.Generic;

namespace CPUTests
{
    public class TestFLAGS_ADD
    {
        private class AddScenario
        {
            public string Name { get; init; } = "";
            public Action<Computer> Setup { get; init; } = _ => { };
            public string Code { get; init; } = "";
            public Action<Computer> AssertResult { get; init; } = _ => { };
        }

        [Fact]
        public void TestFLAG_ZERO_ADD()
        {
            var scenarios = new List<AddScenario>
            {
                new()
                {
                    Name = "8-bit wrap immediate",
                    Setup = c => c.CPU.REGS.A = 0xFF,
                    Code = "ADD A,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((byte)0, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "8-bit wrap absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.A = 0xFF;
                        c.MEMC.Set8bitToRAM(0x9000, 1);
                    },
                    Code = "ADD A,(0x9000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((byte)0, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "16-bit wrap immediate",
                    Setup = c => c.CPU.REGS.AB = 0xFFFF,
                    Code = "ADD AB,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((ushort)0, c.CPU.REGS.AB);
                    }
                },
                new()
                {
                    Name = "24-bit wrap absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0xFFFFFF;
                        c.MEMC.Set24bitToRAM(0xA000, 1);
                    },
                    Code = "ADD ABC,(0xA000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal(0u, c.CPU.REGS.ABC);
                    }
                },
                new()
                {
                    Name = "32-bit wrap immediate",
                    Setup = c => c.CPU.REGS.ABCD = 0xFFFFFFFF,
                    Code = "ADD ABCD,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal(0u, c.CPU.REGS.ABCD);
                    }
                },
                new()
                {
                    Name = "8-bit wrap register to register",
                    Setup = c =>
                    {
                        c.CPU.REGS.A = 0xFF;
                        c.CPU.REGS.B = 0x01;
                    },
                    Code = "ADD A,B\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((byte)0, c.CPU.REGS.A);
                    }
                }
            };

            RunScenarios(scenarios);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestFLAG_CARRY_ADD()
        {
            var scenarios = new List<AddScenario>
            {
                new()
                {
                    Name = "8-bit no carry",
                    Setup = c => c.CPU.REGS.A = 0x10,
                    Code = "ADD A,0x0F\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.False(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((byte)0x1F, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "8-bit carry immediate",
                    Setup = c => c.CPU.REGS.A = 0xF0,
                    Code = "ADD A,0x30\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((byte)0x20, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "16-bit carry immediate",
                    Setup = c => c.CPU.REGS.AB = 0xFFFF,
                    Code = "ADD AB,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((ushort)0x0000, c.CPU.REGS.AB);
                    }
                },
                new()
                {
                    Name = "24-bit carry from absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0xFFFFFF;
                        c.MEMC.Set24bitToRAM(0xC000, 1);
                    },
                    Code = "ADD ABC,(0xC000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0u, c.CPU.REGS.ABC);
                    }
                },
                new()
                {
                    Name = "32-bit no carry immediate",
                    Setup = c => c.CPU.REGS.ABCD = 0x7FFFFFFF,
                    Code = "ADD ABCD,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.False(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0x80000000u, c.CPU.REGS.ABCD);
                    }
                },
                new()
                {
                    Name = "24-bit carry register source",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0xFFFFFF;
                        c.CPU.REGS.DEF = 0x000001;
                    },
                    Code = "ADD ABC,DEF\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0u, c.CPU.REGS.ABC);
                    }
                },
                new()
                {
                    Name = "24-bit no carry register source",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0x123456;
                        c.CPU.REGS.DEF = 0x000001;
                    },
                    Code = "ADD ABC,DEF\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.False(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0x123457u, c.CPU.REGS.ABC);
                    }
                }
            };

            RunScenarios(scenarios);
            TUtils.IncrementCountedTests("exec");
        }

        private static void RunScenarios(IEnumerable<AddScenario> scenarios)
        {
            foreach (var s in scenarios)
            {
                var cp = new Assembler();
                using Computer computer = new();
                s.Setup(computer);
                cp.Build(s.Code);
                computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

                try
                {
                    s.AssertResult(computer);
                }
                catch (Exception ex)
                {
                    throw new Xunit.Sdk.XunitException($"Scenario '{s.Name}' failed: {ex.Message}");
                }
            }
        }
    }
}
