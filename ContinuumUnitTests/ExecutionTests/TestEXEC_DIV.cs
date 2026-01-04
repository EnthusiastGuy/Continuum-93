using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_DIV
    {
        #region Helper Methods

        private static void RunTest(string asm, Action<Computer> arrange, Action<Computer> assert)
        {
            Assembler cp = new();
            using Computer computer = new();

            arrange(computer);

            cp.Build(TUtils.GetFormattedAsm(asm, "BREAK"));
            if (cp.Errors > 0)
                throw new InvalidOperationException($"Assembly failed: {cp.Log}");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            assert(computer);
            TUtils.IncrementCountedTests("exec");
        }

        private static void RunCase(string name, string asm, Action<Computer> arrange, Action<Computer> assert)
        {
            RunTest(asm, arrange, c =>
            {
                try { assert(c); }
                catch (Exception ex) { throw new Xunit.Sdk.XunitException($"{name} failed: {ex.Message}"); }
            });
        }

        #endregion

        #region DIV r,* tests

        [Fact]
        public void DIV_r_variants()
        {
            var cases = new (string name, string asm, Action<Computer> arrange, Action<Computer> assert)[]
            {
                ("DIV r,n", "DIV A,4",
                    c => c.CPU.REGS.A = 100,
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,r", "DIV A,B",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.B = 4; },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(nnn)", "DIV A,(0x4000)",
                    c => { c.CPU.REGS.A = 100; c.MEMC.Set8bitToRAM(0x4000, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(nnn,nnn)", "DIV A,(0x4000+4)",
                    c => { c.CPU.REGS.A = 100; c.MEMC.Set8bitToRAM(0x4004, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(nnn,r)", "DIV A,(0x4000+X)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.X = 3; c.MEMC.Set8bitToRAM(0x4003, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(nnn,rr)", "DIV A,(0x4000+WX)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.WX = 5; c.MEMC.Set8bitToRAM(0x4005, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(nnn,rrr)", "DIV A,(0x4000+VWX)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.VWX = 7; c.MEMC.Set8bitToRAM(0x4007, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(rrr)", "DIV A,(QRS)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set8bitToRAM(0x5000, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(rrr,nnn)", "DIV A,(QRS+4)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set8bitToRAM(0x5004, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(rrr,r)", "DIV A,(QRS+X)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; c.MEMC.Set8bitToRAM(0x5003, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(rrr,rr)", "DIV A,(QRS+WX)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; c.MEMC.Set8bitToRAM(0x5005, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,(rrr,rrr)", "DIV A,(QRS+VWX)",
                    c => { c.CPU.REGS.A = 100; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; c.MEMC.Set8bitToRAM(0x5007, 4); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),

                ("DIV r,fr", "DIV A,F0",
                    c => { c.CPU.REGS.A = 100; c.CPU.FREGS.SetRegister(0, 4.0f); },
                    c => Assert.Equal((byte)25, c.CPU.REGS.A)),
            };

            foreach (var (name, asm, arrange, assert) in cases)
                RunCase(name, asm, arrange, assert);
        }

        #endregion

        #region DIV rr,* tests

        [Fact]
        public void DIV_rr_variants()
        {
            var cases = new (string name, string asm, Action<Computer> arrange, Action<Computer> assert)[]
            {
                ("DIV rr,nn", "DIV AB,0x0004",
                    c => c.CPU.REGS.AB = 1000,
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,r", "DIV AB,E",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.E = 4; },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,rr", "DIV AB,CD",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.CD = 4; },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(nnn)", "DIV AB,(0x4000)",
                    c => { c.CPU.REGS.AB = 1000; c.MEMC.Set16bitToRAM(0x4000, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(nnn,nnn)", "DIV AB,(0x4000+4)",
                    c => { c.CPU.REGS.AB = 1000; c.MEMC.Set16bitToRAM(0x4004, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(nnn,r)", "DIV AB,(0x4000+X)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.X = 3; c.MEMC.Set16bitToRAM(0x4003, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(nnn,rr)", "DIV AB,(0x4000+WX)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.WX = 5; c.MEMC.Set16bitToRAM(0x4005, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(nnn,rrr)", "DIV AB,(0x4000+VWX)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.VWX = 7; c.MEMC.Set16bitToRAM(0x4007, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(rrr)", "DIV AB,(QRS)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set16bitToRAM(0x5000, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(rrr,nnn)", "DIV AB,(QRS+4)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set16bitToRAM(0x5004, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(rrr,r)", "DIV AB,(QRS+X)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; c.MEMC.Set16bitToRAM(0x5003, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(rrr,rr)", "DIV AB,(QRS+WX)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; c.MEMC.Set16bitToRAM(0x5005, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,(rrr,rrr)", "DIV AB,(QRS+VWX)",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; c.MEMC.Set16bitToRAM(0x5007, 4); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),

                ("DIV rr,fr", "DIV AB,F1",
                    c => { c.CPU.REGS.AB = 1000; c.CPU.FREGS.SetRegister(1, 4.0f); },
                    c => Assert.Equal((ushort)250, c.CPU.REGS.AB)),
            };

            foreach (var (name, asm, arrange, assert) in cases)
                RunCase(name, asm, arrange, assert);
        }

        #endregion

        #region DIV rrr,* tests

        [Fact]
        public void DIV_rrr_variants()
        {
            const uint dividend = 100000; // 0x0186A0
            const uint expected = 25000;  // /4

            var cases = new (string name, string asm, Action<Computer> arrange, Action<Computer> assert)[]
            {
                ("DIV rrr,nnn", "DIV ABC,0x000004",
                    c => c.CPU.REGS.ABC = dividend,
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,r", "DIV ABC,D",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.D = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,rr", "DIV ABC,DE",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.DE = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,rrr", "DIV ABC,DEF",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.DEF = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(nnn)", "DIV ABC,(0x4000)",
                    c => { c.CPU.REGS.ABC = dividend; c.MEMC.Set24bitToRAM(0x4000, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(nnn,nnn)", "DIV ABC,(0x4000+4)",
                    c => { c.CPU.REGS.ABC = dividend; c.MEMC.Set24bitToRAM(0x4004, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(nnn,r)", "DIV ABC,(0x4000+X)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.X = 3; c.MEMC.Set24bitToRAM(0x4003, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(nnn,rr)", "DIV ABC,(0x4000+WX)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.WX = 5; c.MEMC.Set24bitToRAM(0x4005, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(nnn,rrr)", "DIV ABC,(0x4000+VWX)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.VWX = 7; c.MEMC.Set24bitToRAM(0x4007, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(rrr)", "DIV ABC,(QRS)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set24bitToRAM(0x5000, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(rrr,nnn)", "DIV ABC,(QRS+4)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set24bitToRAM(0x5004, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(rrr,r)", "DIV ABC,(QRS+X)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; c.MEMC.Set24bitToRAM(0x5003, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(rrr,rr)", "DIV ABC,(QRS+WX)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; c.MEMC.Set24bitToRAM(0x5005, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,(rrr,rrr)", "DIV ABC,(QRS+VWX)",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; c.MEMC.Set24bitToRAM(0x5007, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),

                ("DIV rrr,fr", "DIV ABC,F2",
                    c => { c.CPU.REGS.ABC = dividend; c.CPU.FREGS.SetRegister(2, 4.0f); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABC)),
            };

            foreach (var (name, asm, arrange, assert) in cases)
                RunCase(name, asm, arrange, assert);
        }

        #endregion

        #region DIV rrrr,* tests

        [Fact]
        public void DIV_rrrr_variants()
        {
            const uint dividend = 1_000_000;
            const uint expected = 250_000;

            var cases = new (string name, string asm, Action<Computer> arrange, Action<Computer> assert)[]
            {
                ("DIV rrrr,nnnn", "DIV ABCD,0x00000004",
                    c => c.CPU.REGS.ABCD = dividend,
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,r", "DIV ABCD,E",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.E = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,rr", "DIV ABCD,EF",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.EF = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,rrr", "DIV ABCD,EFG",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.EFG = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,rrrr", "DIV ABCD,EFGH",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.EFGH = 4; },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(nnn)", "DIV ABCD,(0x4000)",
                    c => { c.CPU.REGS.ABCD = dividend; c.MEMC.Set32bitToRAM(0x4000, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(nnn,nnn)", "DIV ABCD,(0x4000+4)",
                    c => { c.CPU.REGS.ABCD = dividend; c.MEMC.Set32bitToRAM(0x4004, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(nnn,r)", "DIV ABCD,(0x4000+X)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.X = 3; c.MEMC.Set32bitToRAM(0x4003, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(nnn,rr)", "DIV ABCD,(0x4000+WX)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.WX = 5; c.MEMC.Set32bitToRAM(0x4005, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(nnn,rrr)", "DIV ABCD,(0x4000+VWX)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.VWX = 7; c.MEMC.Set32bitToRAM(0x4007, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(rrr)", "DIV ABCD,(QRS)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set32bitToRAM(0x5000, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(rrr,nnn)", "DIV ABCD,(QRS+4)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.QRS = 0x5000; c.MEMC.Set32bitToRAM(0x5004, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(rrr,r)", "DIV ABCD,(QRS+X)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; c.MEMC.Set32bitToRAM(0x5003, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(rrr,rr)", "DIV ABCD,(QRS+WX)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; c.MEMC.Set32bitToRAM(0x5005, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,(rrr,rrr)", "DIV ABCD,(QRS+VWX)",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; c.MEMC.Set32bitToRAM(0x5007, 4); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),

                ("DIV rrrr,fr", "DIV ABCD,F3",
                    c => { c.CPU.REGS.ABCD = dividend; c.CPU.FREGS.SetRegister(3, 4.0f); },
                    c => Assert.Equal(expected, c.CPU.REGS.ABCD)),
            };

            foreach (var (name, asm, arrange, assert) in cases)
                RunCase(name, asm, arrange, assert);
        }

        #endregion

        #region DIV memory destination variants (all 10 addressing groups)

        private sealed record MemTarget(string Name, string Asm, Func<Computer, uint> ResolveAddr, Action<Computer> Setup);
        private sealed record MemValue(string Name, string Asm, Func<Computer, uint> ResolveAddr, Action<Computer> Setup);

        private static readonly MemTarget[] _memTargets =
        [
            new("InnnI", "(0x4000)", _ => 0x4000, _ => { }),
            new("Innn_nnnI", "(0x4000+4)", _ => 0x4004, _ => { }),
            // Index registers are architecture-defined: X (8-bit), WX (16-bit), VWX (24-bit).
            // These overlap, so tests that combine different addressing modes compute effective
            // addresses dynamically rather than assuming fixed offsets.
            new("Innn_rI", "(0x4000+X)", c => (uint)(0x4000 + c.CPU.REGS.X), c => c.CPU.REGS.X = 3),
            new("Innn_rrI", "(0x4000+WX)", c => (uint)(0x4000 + c.CPU.REGS.WX), c => c.CPU.REGS.WX = 5),
            new("Innn_rrrI", "(0x4000+VWX)", c => (uint)(0x4000 + c.CPU.REGS.VWX), c => c.CPU.REGS.VWX = 7),

            new("IrrrI", "(QRS)", c => c.CPU.REGS.QRS, c => c.CPU.REGS.QRS = 0x5000),
            new("Irrr_nnnI", "(QRS+4)", c => c.CPU.REGS.QRS + 4, c => c.CPU.REGS.QRS = 0x5000),
            new("Irrr_rI", "(QRS+X)", c => c.CPU.REGS.QRS + c.CPU.REGS.X, c => { c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.X = 3; }),
            new("Irrr_rrI", "(QRS+WX)", c => c.CPU.REGS.QRS + c.CPU.REGS.WX, c => { c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.WX = 5; }),
            new("Irrr_rrrI", "(QRS+VWX)", c => c.CPU.REGS.QRS + c.CPU.REGS.VWX, c => { c.CPU.REGS.QRS = 0x5000; c.CPU.REGS.VWX = 7; }),
        ];

        private static readonly MemValue[] _memValues =
        [
            new("InnnI", "(0x7000)", _ => 0x7000, _ => { }),
            new("Innn_nnnI", "(0x7000+4)", _ => 0x7004, _ => { }),
            new("Innn_rI", "(0x7000+X)", c => (uint)(0x7000 + c.CPU.REGS.X), c => c.CPU.REGS.X = 3),
            new("Innn_rrI", "(0x7000+WX)", c => (uint)(0x7000 + c.CPU.REGS.WX), c => c.CPU.REGS.WX = 5),
            new("Innn_rrrI", "(0x7000+VWX)", c => (uint)(0x7000 + c.CPU.REGS.VWX), c => c.CPU.REGS.VWX = 7),

            new("IrrrI", "(TUV)", c => c.CPU.REGS.TUV, c => c.CPU.REGS.TUV = 0x8000),
            new("Irrr_nnnI", "(TUV+4)", c => c.CPU.REGS.TUV + 4, c => c.CPU.REGS.TUV = 0x8000),
            new("Irrr_rI", "(TUV+X)", c => c.CPU.REGS.TUV + c.CPU.REGS.X, c => { c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.X = 3; }),
            new("Irrr_rrI", "(TUV+WX)", c => c.CPU.REGS.TUV + c.CPU.REGS.WX, c => { c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.WX = 5; }),
            new("Irrr_rrrI", "(TUV+VWX)", c => c.CPU.REGS.TUV + c.CPU.REGS.VWX, c => { c.CPU.REGS.TUV = 0x8000; c.CPU.REGS.VWX = 7; }),
        ];

        [Fact]
        public void DIV_memory_destination_variants()
        {
            foreach (var target in _memTargets)
            {
                uint addr = 0;
                // block immediate
                RunTest(
                    $"DIV {target.Asm},0x00000002,2",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.LoadMemAt(addr, [0x00, 0x40]); // 64 (big-endian) -> /2 => 32
                    },
                    c => Assert.Equal(new byte[] { 0x00, 0x20 }, c.MEMC.RAM.GetMemoryAt(addr, 2)));

                // block immediate with repeat (divide twice)
                RunTest(
                    $"DIV {target.Asm},0x00000002,2,2",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.LoadMemAt(addr, [0x00, 0x40]); // 64 -> /2 -> /2 => 16
                    },
                    c => Assert.Equal(new byte[] { 0x00, 0x10 }, c.MEMC.RAM.GetMemoryAt(addr, 2)));

                // (dest),r
                RunTest(
                    $"DIV {target.Asm},B",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.MEMC.Set8bitToRAM(addr, 100);
                        c.CPU.REGS.B = 4;
                    },
                    c => Assert.Equal((byte)25, c.MEMC.Get8bitFromRAM(addr)));

                // (dest),rr
                RunTest(
                    $"DIV {target.Asm},CD",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.MEMC.Set16bitToRAM(addr, 1000);
                        c.CPU.REGS.CD = 4;
                    },
                    c => Assert.Equal((ushort)250, c.MEMC.Get16bitFromRAM(addr)));

                // (dest),rrr
                RunTest(
                    $"DIV {target.Asm},DEF",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.MEMC.Set24bitToRAM(addr, 100000);
                        c.CPU.REGS.DEF = 4;
                    },
                    c => Assert.Equal((uint)25000, c.MEMC.Get24bitFromRAM(addr)));

                // (dest),rrrr
                RunTest(
                    $"DIV {target.Asm},EFGH",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.MEMC.Set32bitToRAM(addr, 1_000_000);
                        c.CPU.REGS.EFGH = 4;
                    },
                    c => Assert.Equal((uint)250_000, c.MEMC.Get32bitFromRAM(addr)));

                // (dest),(value),n,rrr : 64 divided by 2 twice => 16
                foreach (var value in _memValues)
                {
                    uint valueAddr = 0;
                    RunCase(
                        $"DIV {target.Name},{value.Name} block",
                        $"DIV {target.Asm},{value.Asm},2,GHI",
                        c =>
                        {
                            c.CPU.REGS.GHI = 2;
                            target.Setup(c);
                            value.Setup(c);

                            addr = target.ResolveAddr(c);
                            valueAddr = value.ResolveAddr(c);

                            c.LoadMemAt(addr, [0x00, 0x40]);
                            c.LoadMemAt(valueAddr, [0x00, 0x02]);
                        },
                        c => Assert.Equal(new byte[] { 0x00, 0x10 }, c.MEMC.RAM.GetMemoryAt(addr, 2)));
                }

                // (dest),fr : float at memory divided by float reg
                RunTest(
                    $"DIV {target.Asm},F0",
                    c =>
                    {
                        target.Setup(c);
                        addr = target.ResolveAddr(c);
                        c.MEMC.SetFloatToRam(addr, 100.0f);
                        c.CPU.FREGS.SetRegister(0, 4.0f);
                    },
                    c => Assert.Equal(25.0f, c.MEMC.GetFloatFromRAM(addr)));
            }
        }

        #endregion

        #region DIV fr,* tests (16 sub-ops)

        [Fact]
        public void DIV_fr_variants()
        {
            var cases = new (string name, string asm, Action<Computer> arrange, Action<Computer> assert)[]
            {
                ("DIV fr,nnnn", "DIV F0,4.0",
                    c => c.CPU.FREGS.SetRegister(0, 100.0f),
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,r", "DIV F0,A",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.A = 4; },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,rr", "DIV F0,AB",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.AB = 4; },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,rrr", "DIV F0,ABC",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.ABC = 4; },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,rrrr", "DIV F0,ABCD",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.ABCD = 4; },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(nnn)", "DIV F0,(0x9000)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.MEMC.SetFloatToRam(0x9000, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(nnn,nnn)", "DIV F0,(0x9000+4)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.MEMC.SetFloatToRam(0x9004, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(nnn,r)", "DIV F0,(0x9000+X)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.X = 3; c.MEMC.SetFloatToRam(0x9003, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(nnn,rr)", "DIV F0,(0x9000+WX)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.WX = 5; c.MEMC.SetFloatToRam(0x9005, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(nnn,rrr)", "DIV F0,(0x9000+VWX)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.VWX = 7; c.MEMC.SetFloatToRam(0x9007, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(rrr)", "DIV F0,(QRS)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.QRS = 0xA000; c.MEMC.SetFloatToRam(0xA000, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(rrr,nnn)", "DIV F0,(QRS+4)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.QRS = 0xA000; c.MEMC.SetFloatToRam(0xA004, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(rrr,r)", "DIV F0,(QRS+X)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.QRS = 0xA000; c.CPU.REGS.X = 3; c.MEMC.SetFloatToRam(0xA003, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(rrr,rr)", "DIV F0,(QRS+WX)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.QRS = 0xA000; c.CPU.REGS.WX = 5; c.MEMC.SetFloatToRam(0xA005, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,(rrr,rrr)", "DIV F0,(QRS+VWX)",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.REGS.QRS = 0xA000; c.CPU.REGS.VWX = 7; c.MEMC.SetFloatToRam(0xA007, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),

                ("DIV fr,fr", "DIV F0,F1",
                    c => { c.CPU.FREGS.SetRegister(0, 100.0f); c.CPU.FREGS.SetRegister(1, 4.0f); },
                    c => Assert.Equal(25.0f, c.CPU.FREGS.GetRegister(0))),
            };

            foreach (var (name, asm, arrange, assert) in cases)
                RunCase(name, asm, arrange, assert);
        }

        #endregion
    }
}

