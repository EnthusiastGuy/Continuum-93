using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using System;
using System.Collections.Generic;

namespace CPUTests
{
    public class TestFLAGS_SUB
    {
        private class SubScenario
        {
            public string Name { get; init; } = "";
            public Action<Computer> Setup { get; init; } = _ => { };
            public string Code { get; init; } = "";
            public Action<Computer> AssertResult { get; init; } = _ => { };
        }

        [Fact]
        public void TestFLAG_ZERO_SUB()
        {
            var scenarios = new List<SubScenario>
            {
                new()
                {
                    Name = "8-bit zero immediate",
                    Setup = c => c.CPU.REGS.A = 0x01,
                    Code = "SUB A,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((byte)0, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "8-bit zero absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.A = 0x01;
                        c.MEMC.Set8bitToRAM(0x9000, 0x01);
                    },
                    Code = "SUB A,(0x9000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((byte)0, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "16-bit zero immediate",
                    Setup = c => c.CPU.REGS.AB = 0x0001,
                    Code = "SUB AB,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal((ushort)0, c.CPU.REGS.AB);
                    }
                },
                new()
                {
                    Name = "24-bit zero absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0x000001;
                        c.MEMC.Set24bitToRAM(0xA000, 0x000001);
                    },
                    Code = "SUB ABC,(0xA000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal(0u, c.CPU.REGS.ABC);
                    }
                },
                new()
                {
                    Name = "32-bit zero immediate",
                    Setup = c => c.CPU.REGS.ABCD = 0x00000001,
                    Code = "SUB ABCD,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsZero());
                        Assert.Equal(0u, c.CPU.REGS.ABCD);
                    }
                }
            };

            RunScenarios(scenarios);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestFLAG_CARRY_SUB()
        {
            var scenarios = new List<SubScenario>
            {
                new()
                {
                    Name = "8-bit borrow immediate",
                    Setup = c => c.CPU.REGS.A = 0x10,
                    Code = "SUB A,0x20\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((byte)0xF0, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "8-bit no borrow",
                    Setup = c => c.CPU.REGS.A = 0x10,
                    Code = "SUB A,0x0F\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.False(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((byte)0x01, c.CPU.REGS.A);
                    }
                },
                new()
                {
                    Name = "16-bit borrow immediate",
                    Setup = c => c.CPU.REGS.AB = 0x0000,
                    Code = "SUB AB,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal((ushort)0xFFFF, c.CPU.REGS.AB);
                    }
                },
                new()
                {
                    Name = "24-bit borrow absolute",
                    Setup = c =>
                    {
                        c.CPU.REGS.ABC = 0x000000;
                        c.MEMC.Set24bitToRAM(0xB000, 0x000001);
                    },
                    Code = "SUB ABC,(0xB000)\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.True(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0xFFFFFFu, c.CPU.REGS.ABC);
                    }
                },
                new()
                {
                    Name = "32-bit no borrow immediate",
                    Setup = c => c.CPU.REGS.ABCD = 0x80000000,
                    Code = "SUB ABCD,1\nBREAK",
                    AssertResult = c =>
                    {
                        Assert.False(c.CPU.FLAGS.IsCarry());
                        Assert.Equal(0x7FFFFFFFu, c.CPU.REGS.ABCD);
                    }
                }
            };

            RunScenarios(scenarios);
            TUtils.IncrementCountedTests("exec");
        }

        private static void RunScenarios(IEnumerable<SubScenario> scenarios)
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
