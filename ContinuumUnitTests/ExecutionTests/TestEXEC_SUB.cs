using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;

namespace ExecutionTests
{
    public class TestEXEC_SUB
    {
        private const uint TargetAbsBase = 0x4000;
        private const uint TargetPtrBase = 0x5000;
        private const uint ValueAbsBase = 0x7000;
        private const uint ValuePtrBase = 0x7100;

        private const int ImmediateOffset = 4;
        private const byte TargetReg8OffsetIndex = 23;   // X
        private const byte TargetReg8OffsetValue = 3;
        private const byte TargetReg16OffsetIndex = 22;  // W X
        private const ushort TargetReg16OffsetValue = 5;
        private const byte TargetReg24OffsetIndex = 21;  // V W X
        private const uint TargetReg24OffsetValue = 7;
        // Use a pointer register that does not overlap with any source register used in the tests.
        // Use disjoint register windows so target pointers never overlap source regs.
        private const byte TargetPtrIndex = 16;          // Q R S

        private const byte ValueReg8OffsetIndex = 10;    // K
        private const byte ValueReg8OffsetValue = 2;
        private const byte ValueReg16OffsetIndex = 10;   // K L
        private const ushort ValueReg16OffsetValue = 6;
        private const byte ValueReg24OffsetIndex = 10;   // K L M
        private const uint ValueReg24OffsetValue = 9;
        // Use another disjoint window for value-side pointers; avoid overlap with offset regs.
        private const byte ValuePtrIndex = 3;            // D E F
        // Repeat register must not overlap with ANY target/value offset or pointer registers.
        // Target offsets use 21-23 (VWX), so RepeatReg must be elsewhere.
        private const byte RepeatRegIndex = 6;           // G H I

        public record AddressMode(string Label, string Operand, Action<Computer> Prime, Func<Computer, uint> Resolve);

        private static uint MaskForWidth(int widthBytes) =>
            widthBytes switch
            {
                1 => 0xFFu,
                2 => 0xFFFFu,
                3 => 0xFFFFFFu,
                _ => 0xFFFFFFFFu
            };

        private static uint SubMasked(uint a, uint b, int widthBytes)
        {
            uint mask = MaskForWidth(widthBytes);
            return (a - b) & mask;
        }

        private static IEnumerable<AddressMode> TargetAddressModes()
        {
            yield return new AddressMode("(nnn)", $"({FormatHex(TargetAbsBase)})",
                _ => { }, _ => TargetAbsBase);

            yield return new AddressMode("(nnn,nnn)", $"({FormatHex(TargetAbsBase)}+{ImmediateOffset})",
                _ => { }, _ => TargetAbsBase + ImmediateOffset);

            yield return new AddressMode("(nnn,r)", $"({FormatHex(TargetAbsBase)}+{TUtils.Get8bitRegisterChar(TargetReg8OffsetIndex)})",
                c => c.CPU.REGS.Set8BitRegister(TargetReg8OffsetIndex, TargetReg8OffsetValue),
                c => TargetAbsBase + TargetReg8OffsetValue);

            yield return new AddressMode("(nnn,rr)", $"({FormatHex(TargetAbsBase)}+{TUtils.Get16bitRegisterString(TargetReg16OffsetIndex)})",
                c => c.CPU.REGS.Set16BitRegister(TargetReg16OffsetIndex, TargetReg16OffsetValue),
                c => TargetAbsBase + TargetReg16OffsetValue);

            yield return new AddressMode("(nnn,rrr)", $"({FormatHex(TargetAbsBase)}+{TUtils.Get24bitRegisterString(TargetReg24OffsetIndex)})",
                c => c.CPU.REGS.Set24BitRegister(TargetReg24OffsetIndex, TargetReg24OffsetValue),
                c => TargetAbsBase + TargetReg24OffsetValue);

            yield return new AddressMode("(rrr)", $"({TUtils.Get24bitRegisterString(TargetPtrIndex)})",
                c => c.CPU.REGS.Set24BitRegister(TargetPtrIndex, TargetPtrBase),
                c => TargetPtrBase);

            yield return new AddressMode("(rrr,nnn)", $"({TUtils.Get24bitRegisterString(TargetPtrIndex)}+{ImmediateOffset})",
                c => c.CPU.REGS.Set24BitRegister(TargetPtrIndex, TargetPtrBase),
                c => TargetPtrBase + ImmediateOffset);

            yield return new AddressMode("(rrr,r)", $"({TUtils.Get24bitRegisterString(TargetPtrIndex)}+{TUtils.Get8bitRegisterChar(TargetReg8OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(TargetPtrIndex, TargetPtrBase);
                    c.CPU.REGS.Set8BitRegister(TargetReg8OffsetIndex, TargetReg8OffsetValue);
                },
                c => TargetPtrBase + TargetReg8OffsetValue);

            yield return new AddressMode("(rrr,rr)", $"({TUtils.Get24bitRegisterString(TargetPtrIndex)}+{TUtils.Get16bitRegisterString(TargetReg16OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(TargetPtrIndex, TargetPtrBase);
                    c.CPU.REGS.Set16BitRegister(TargetReg16OffsetIndex, TargetReg16OffsetValue);
                },
                c => TargetPtrBase + TargetReg16OffsetValue);

            yield return new AddressMode("(rrr,rrr)", $"({TUtils.Get24bitRegisterString(TargetPtrIndex)}+{TUtils.Get24bitRegisterString(TargetReg24OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(TargetPtrIndex, TargetPtrBase);
                    c.CPU.REGS.Set24BitRegister(TargetReg24OffsetIndex, TargetReg24OffsetValue);
                },
                c => TargetPtrBase + TargetReg24OffsetValue);
        }

        private static IEnumerable<AddressMode> ValueAddressModes()
        {
            yield return new AddressMode("(nnn)", $"({FormatHex(ValueAbsBase)})",
                _ => { }, _ => ValueAbsBase);

            yield return new AddressMode("(nnn,nnn)", $"({FormatHex(ValueAbsBase)}+{ImmediateOffset})",
                _ => { }, _ => ValueAbsBase + ImmediateOffset);

            yield return new AddressMode("(nnn,r)", $"({FormatHex(ValueAbsBase)}+{TUtils.Get8bitRegisterChar(ValueReg8OffsetIndex)})",
                c => c.CPU.REGS.Set8BitRegister(ValueReg8OffsetIndex, ValueReg8OffsetValue),
                c => ValueAbsBase + ValueReg8OffsetValue);

            yield return new AddressMode("(nnn,rr)", $"({FormatHex(ValueAbsBase)}+{TUtils.Get16bitRegisterString(ValueReg16OffsetIndex)})",
                c => c.CPU.REGS.Set16BitRegister(ValueReg16OffsetIndex, ValueReg16OffsetValue),
                c => ValueAbsBase + ValueReg16OffsetValue);

            yield return new AddressMode("(nnn,rrr)", $"({FormatHex(ValueAbsBase)}+{TUtils.Get24bitRegisterString(ValueReg24OffsetIndex)})",
                c => c.CPU.REGS.Set24BitRegister(ValueReg24OffsetIndex, ValueReg24OffsetValue),
                c => ValueAbsBase + ValueReg24OffsetValue);

            yield return new AddressMode("(rrr)", $"({TUtils.Get24bitRegisterString(ValuePtrIndex)})",
                c => c.CPU.REGS.Set24BitRegister(ValuePtrIndex, ValuePtrBase),
                c => ValuePtrBase);

            yield return new AddressMode("(rrr,nnn)", $"({TUtils.Get24bitRegisterString(ValuePtrIndex)}+{ImmediateOffset})",
                c => c.CPU.REGS.Set24BitRegister(ValuePtrIndex, ValuePtrBase),
                c => ValuePtrBase + ImmediateOffset);

            yield return new AddressMode("(rrr,r)", $"({TUtils.Get24bitRegisterString(ValuePtrIndex)}+{TUtils.Get8bitRegisterChar(ValueReg8OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(ValuePtrIndex, ValuePtrBase);
                    c.CPU.REGS.Set8BitRegister(ValueReg8OffsetIndex, ValueReg8OffsetValue);
                },
                c => ValuePtrBase + ValueReg8OffsetValue);

            yield return new AddressMode("(rrr,rr)", $"({TUtils.Get24bitRegisterString(ValuePtrIndex)}+{TUtils.Get16bitRegisterString(ValueReg16OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(ValuePtrIndex, ValuePtrBase);
                    c.CPU.REGS.Set16BitRegister(ValueReg16OffsetIndex, ValueReg16OffsetValue);
                },
                c => ValuePtrBase + ValueReg16OffsetValue);

            yield return new AddressMode("(rrr,rrr)", $"({TUtils.Get24bitRegisterString(ValuePtrIndex)}+{TUtils.Get24bitRegisterString(ValueReg24OffsetIndex)})",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(ValuePtrIndex, ValuePtrBase);
                    c.CPU.REGS.Set24BitRegister(ValueReg24OffsetIndex, ValueReg24OffsetValue);
                },
                c => ValuePtrBase + ValueReg24OffsetValue);
        }

        #region Register destination cases

        public static IEnumerable<object[]> SubByteRegisterCases()
        {
            byte destIndex = 0; // A
            byte destInitial = 50;

            yield return CreateRegisterCase(
                "SUB r,n",
                $"SUB {TUtils.Get8bitRegisterChar(destIndex)},5",
                c => c.CPU.REGS.Set8BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get8BitRegister(destIndex),
                (uint)(destInitial - 5));

            yield return CreateRegisterCase(
                "SUB r,nnn",
                $"SUB {TUtils.Get8bitRegisterChar(destIndex)},30",
                c => c.CPU.REGS.Set8BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get8BitRegister(destIndex),
                (uint)(destInitial - 30));

            yield return CreateRegisterCase(
                "SUB r,r",
                $"SUB {TUtils.Get8bitRegisterChar(destIndex)},{TUtils.Get8bitRegisterChar(1)}",
                c =>
                {
                    c.CPU.REGS.Set8BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set8BitRegister(1, 3);
                },
                comp => comp.CPU.REGS.Get8BitRegister(destIndex),
                (uint)(destInitial - 3));

            foreach (var mode in TargetAddressModes())
            {
                byte source = 0x0A;
                yield return CreateRegisterCase(
                    $"SUB r,{mode.Label}",
                    $"SUB {TUtils.Get8bitRegisterChar(destIndex)},{mode.Operand}",
                    c =>
                    {
                        mode.Prime(c);
                        c.MEMC.Set8bitToRAM(mode.Resolve(c), source);
                        c.CPU.REGS.Set8BitRegister(destIndex, destInitial);
                    },
                    comp => comp.CPU.REGS.Get8BitRegister(destIndex),
                    (uint)(destInitial - source));
            }

            yield return CreateRegisterCase(
                "SUB r,fr",
                $"SUB {TUtils.Get8bitRegisterChar(destIndex)},F0",
                c =>
                {
                    c.CPU.REGS.Set8BitRegister(destIndex, destInitial);
                    c.CPU.FREGS.SetRegister(0, 1.5f);
                },
                comp => comp.CPU.REGS.Get8BitRegister(destIndex),
                (uint)Math.Round(destInitial - 1.5f));
        }

        public static IEnumerable<object[]> SubWordRegisterCases()
        {
            byte destIndex = 0; // AB
            ushort destInitial = 0x5000;

            yield return CreateRegisterCase(
                "SUB rr,nn",
                $"SUB {TUtils.Get16bitRegisterString(destIndex)},0x1234",
                c => c.CPU.REGS.Set16BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                SubMasked(destInitial, 0x1234, 2));

            yield return CreateRegisterCase(
                "SUB rr,nnn",
                $"SUB {TUtils.Get16bitRegisterString(destIndex)},0x0020",
                c => c.CPU.REGS.Set16BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                SubMasked(destInitial, 0x20, 2));

            yield return CreateRegisterCase(
                "SUB rr,r",
                $"SUB {TUtils.Get16bitRegisterString(destIndex)},{TUtils.Get8bitRegisterChar(4)}",
                c =>
                {
                    c.CPU.REGS.Set16BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set8BitRegister(4, 4);
                },
                comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                SubMasked(destInitial, 4, 2));

            yield return CreateRegisterCase(
                "SUB rr,rr",
                $"SUB {TUtils.Get16bitRegisterString(destIndex)},{TUtils.Get16bitRegisterString(2)}",
                c =>
                {
                    c.CPU.REGS.Set16BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set16BitRegister(2, 0x00FF);
                },
                comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                SubMasked(destInitial, 0x00FF, 2));

            foreach (var mode in TargetAddressModes())
            {
                ushort source = 0x0100;
                yield return CreateRegisterCase(
                    $"SUB rr,{mode.Label}",
                    $"SUB {TUtils.Get16bitRegisterString(destIndex)},{mode.Operand}",
                    c =>
                    {
                        mode.Prime(c);
                        c.MEMC.Set16bitToRAM(mode.Resolve(c), source);
                        c.CPU.REGS.Set16BitRegister(destIndex, destInitial);
                    },
                    comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                    SubMasked(destInitial, source, 2));
            }

            yield return CreateRegisterCase(
                "SUB rr,fr",
                $"SUB {TUtils.Get16bitRegisterString(destIndex)},F1",
                c =>
                {
                    c.CPU.REGS.Set16BitRegister(destIndex, destInitial);
                    c.CPU.FREGS.SetRegister(1, 5.6f);
                },
                comp => comp.CPU.REGS.Get16BitRegister(destIndex),
                (uint)Math.Round(destInitial - 5.6f) & 0xFFFFu);
        }

        public static IEnumerable<object[]> SubTriByteRegisterCases()
        {
            byte destIndex = 0; // ABC
            uint destInitial = 0x050000;

            yield return CreateRegisterCase(
                "SUB rrr,nnn",
                $"SUB {TUtils.Get24bitRegisterString(destIndex)},0x000102",
                c => c.CPU.REGS.Set24BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                SubMasked(destInitial, 0x000102, 3));

            yield return CreateRegisterCase(
                "SUB rrr,r",
                $"SUB {TUtils.Get24bitRegisterString(destIndex)},{TUtils.Get8bitRegisterChar(4)}",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set8BitRegister(4, 7);
                },
                comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                SubMasked(destInitial, 7, 3));

            yield return CreateRegisterCase(
                "SUB rrr,rr",
                $"SUB {TUtils.Get24bitRegisterString(destIndex)},{TUtils.Get16bitRegisterString(4)}",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set16BitRegister(4, 0x0102);
                },
                comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                SubMasked(destInitial, 0x0102, 3));

            yield return CreateRegisterCase(
                "SUB rrr,rrr",
                $"SUB {TUtils.Get24bitRegisterString(destIndex)},{TUtils.Get24bitRegisterString(3)}",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set24BitRegister(3, 0x00000F);
                },
                comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                SubMasked(destInitial, 0x00000F, 3));

            foreach (var mode in TargetAddressModes())
            {
                uint source = 0x001000;
                yield return CreateRegisterCase(
                    $"SUB rrr,{mode.Label}",
                    $"SUB {TUtils.Get24bitRegisterString(destIndex)},{mode.Operand}",
                    c =>
                    {
                        mode.Prime(c);
                        c.MEMC.Set24bitToRAM(mode.Resolve(c), source);
                        c.CPU.REGS.Set24BitRegister(destIndex, destInitial);
                    },
                    comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                    SubMasked(destInitial, source, 3));
            }

            yield return CreateRegisterCase(
                "SUB rrr,fr",
                $"SUB {TUtils.Get24bitRegisterString(destIndex)},F2",
                c =>
                {
                    c.CPU.REGS.Set24BitRegister(destIndex, destInitial);
                    c.CPU.FREGS.SetRegister(2, 12.5f);
                },
                comp => comp.CPU.REGS.Get24BitRegister(destIndex),
                SubMasked(destInitial, (uint)Math.Round(12.5f), 3));
        }

        public static IEnumerable<object[]> SubDWordRegisterCases()
        {
            byte destIndex = 0; // ABCD
            uint destInitial = 0x5000_0000;

            yield return CreateRegisterCase(
                "SUB rrrr,nnnn",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},0x00010002",
                c => c.CPU.REGS.Set32BitRegister(destIndex, destInitial),
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, 0x00010002, 4));

            yield return CreateRegisterCase(
                "SUB rrrr,r",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},{TUtils.Get8bitRegisterChar(4)}",
                c =>
                {
                    c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set8BitRegister(4, 9);
                },
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, 9, 4));

            yield return CreateRegisterCase(
                "SUB rrrr,rr",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},{TUtils.Get16bitRegisterString(4)}",
                c =>
                {
                    c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set16BitRegister(4, 0x00FF);
                },
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, 0x00FF, 4));

            yield return CreateRegisterCase(
                "SUB rrrr,rrr",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},{TUtils.Get24bitRegisterString(3)}",
                c =>
                {
                    c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set24BitRegister(3, 0x0000AA);
                },
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, 0x0000AA, 4));

            yield return CreateRegisterCase(
                "SUB rrrr,rrrr",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},{TUtils.Get32bitRegisterString(4)}",
                c =>
                {
                    c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    c.CPU.REGS.Set32BitRegister(4, 0x00000010);
                },
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, 0x00000010, 4));

            foreach (var mode in TargetAddressModes())
            {
                uint source = 0x0000_0100;
                yield return CreateRegisterCase(
                    $"SUB rrrr,{mode.Label}",
                    $"SUB {TUtils.Get32bitRegisterString(destIndex)},{mode.Operand}",
                    c =>
                    {
                        mode.Prime(c);
                        c.MEMC.Set32bitToRAM(mode.Resolve(c), source);
                        c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    },
                    comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                    SubMasked(destInitial, source, 4));
            }

            yield return CreateRegisterCase(
                "SUB rrrr,fr",
                $"SUB {TUtils.Get32bitRegisterString(destIndex)},F3",
                c =>
                {
                    c.CPU.REGS.Set32BitRegister(destIndex, destInitial);
                    c.CPU.FREGS.SetRegister(3, 42.8f);
                },
                comp => comp.CPU.REGS.Get32BitRegister(destIndex),
                SubMasked(destInitial, (uint)Math.Round(42.8f), 4));
        }

        #endregion

        #region Memory destination cases

        public static IEnumerable<object[]> MemoryImmediateCases()
        {
            byte countShort = 2;
            byte countLong = 3;
            foreach (var target in TargetAddressModes())
            {
                yield return new object[]
                {
                    $"SUB {target.Label} immediate once",
                    target,
                    0x00010203u,
                    countShort,
                    1u
                };

                yield return new object[]
                {
                    $"SUB {target.Label} immediate with repeat",
                    target,
                    0x00020304u,
                    countLong,
                    2u
                };
            }
        }

        public static IEnumerable<object[]> MemoryRegisterCases()
        {
            foreach (var target in TargetAddressModes())
            {
                yield return new object[] { $"SUB {target.Label},r", target, 1, (Func<Computer, uint>)(c => c.CPU.REGS.Get8BitRegister(1)), (Action<Computer>)(c => c.CPU.REGS.Set8BitRegister(1, 5)) };
                yield return new object[] { $"SUB {target.Label},rr", target, 2, (Func<Computer, uint>)(c => c.CPU.REGS.Get16BitRegister(2)), (Action<Computer>)(c => c.CPU.REGS.Set16BitRegister(2, 0x0102)) };
                yield return new object[] { $"SUB {target.Label},rrr", target, 3, (Func<Computer, uint>)(c => c.CPU.REGS.Get24BitRegister(3)), (Action<Computer>)(c => c.CPU.REGS.Set24BitRegister(3, 0x000203)) };
                yield return new object[] { $"SUB {target.Label},rrrr", target, 4, (Func<Computer, uint>)(c => c.CPU.REGS.Get32BitRegister(4)), (Action<Computer>)(c => c.CPU.REGS.Set32BitRegister(4, 0x00030405)) };
            }
        }

        public static IEnumerable<object[]> MemoryValueBlockCases()
        {
            byte count = 3;
            uint repeat = 2;
            var valueModes = ValueAddressModes().ToList();
            foreach (var target in TargetAddressModes())
            {
                foreach (var value in valueModes)
                {
                    yield return new object[]
                    {
                        $"SUB {target.Label},{value.Label} block",
                        target,
                        value,
                        count,
                        repeat
                    };
                }
            }
        }

        public static IEnumerable<object[]> MemoryFloatCases()
        {
            foreach (var target in TargetAddressModes())
            {
                yield return new object[]
                {
                    $"SUB {target.Label},fr",
                    target
                };
            }
        }

        #endregion

        #region Float register destination addressing

        public static IEnumerable<object[]> FloatRegisterCases()
        {
            byte destIndex = 0; // F0
            float destInitial = 100.5f;

            // SUB fr,nnn - 24-bit immediate as float
            yield return CreateFloatRegisterCase(
                "SUB fr,nnn",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},0x40A00000",
                c => c.CPU.FREGS.SetRegister(destIndex, destInitial),
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 5.0f); // 0x40A00000 = 5.0f in IEEE 754

            // SUB fr,nnnn - 32-bit immediate as float
            yield return CreateFloatRegisterCase(
                "SUB fr,nnnn",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},0x40C00000",
                c => c.CPU.FREGS.SetRegister(destIndex, destInitial),
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 6.0f); // 0x40C00000 = 6.0f in IEEE 754

            // SUB fr,r
            yield return CreateFloatRegisterCase(
                "SUB fr,r",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{TUtils.Get8bitRegisterChar(9)}",
                c =>
                {
                    c.CPU.FREGS.SetRegister(destIndex, destInitial);
                    c.CPU.REGS.Set8BitRegister(9, 3);
                },
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 3.0f);

            // SUB fr,rr
            yield return CreateFloatRegisterCase(
                "SUB fr,rr",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{TUtils.Get16bitRegisterString(9)}",
                c =>
                {
                    c.CPU.FREGS.SetRegister(destIndex, destInitial);
                    c.CPU.REGS.Set16BitRegister(9, 1000);
                },
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 1000.0f);

            // SUB fr,rrr
            yield return CreateFloatRegisterCase(
                "SUB fr,rrr",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{TUtils.Get24bitRegisterString(9)}",
                c =>
                {
                    c.CPU.FREGS.SetRegister(destIndex, destInitial);
                    c.CPU.REGS.Set24BitRegister(9, 50000);
                },
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 50000.0f);

            // SUB fr,rrrr
            yield return CreateFloatRegisterCase(
                "SUB fr,rrrr",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{TUtils.Get32bitRegisterString(9)}",
                c =>
                {
                    c.CPU.FREGS.SetRegister(destIndex, destInitial);
                    c.CPU.REGS.Set32BitRegister(9, 1000);
                },
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 1000.0f);

            // SUB fr,fr
            yield return CreateFloatRegisterCase(
                "SUB fr,fr",
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{TUtils.GetFloatRegisterString(1)}",
                c =>
                {
                    c.CPU.FREGS.SetRegister(destIndex, destInitial);
                    c.CPU.FREGS.SetRegister(1, 2.75f);
                },
                comp => comp.CPU.FREGS.GetRegister(destIndex),
                destInitial - 2.75f);
        }

        public static IEnumerable<object[]> FloatRegisterMemoryCases()
        {
            byte destIndex = 0;
            foreach (var mode in TargetAddressModes())
            {
                yield return new object[]
                {
                    $"SUB fr,{mode.Label}",
                    mode,
                    destIndex
                };
            }
        }

        #endregion

        #region Tests

        [Theory]
        [MemberData(nameof(SubByteRegisterCases))]
        public void SUB_r_variations(string name, string asm, Action<Computer> arrange, Func<Computer, uint> actual, uint expected)
        {
            RunAsmCase(name, asm, arrange, c => Assert.Equal(expected, actual(c)));
        }

        [Theory]
        [MemberData(nameof(SubWordRegisterCases))]
        public void SUB_rr_variations(string name, string asm, Action<Computer> arrange, Func<Computer, uint> actual, uint expected)
        {
            RunAsmCase(name, asm, arrange, c => Assert.Equal(expected, actual(c)));
        }

        [Theory]
        [MemberData(nameof(SubTriByteRegisterCases))]
        public void SUB_rrr_variations(string name, string asm, Action<Computer> arrange, Func<Computer, uint> actual, uint expected)
        {
            RunAsmCase(name, asm, arrange, c => Assert.Equal(expected, actual(c)));
        }

        [Theory]
        [MemberData(nameof(SubDWordRegisterCases))]
        public void SUB_rrrr_variations(string name, string asm, Action<Computer> arrange, Func<Computer, uint> actual, uint expected)
        {
            RunAsmCase(name, asm, arrange, c => Assert.Equal(expected, actual(c)));
        }

        [Theory]
        [MemberData(nameof(MemoryImmediateCases))]
        public void SUB_memory_immediate(string name, AddressMode target, uint value, byte count, uint repeat)
        {
            byte[] seed = new byte[] { 10, 20, 30, 40, 50 };
            byte[] initial = seed.Take(count).ToArray();

            RunAsmCase(
                name,
                $"SUB {target.Operand},{FormatHex(value)},{count}" + (repeat > 1 ? $",{repeat}" : string.Empty),
                c =>
                {
                    target.Prime(c);
                    PrepareMemorySpan(c, target.Resolve(c), count, seed);
                },
                c =>
                {
                    byte[] expected = ComputeBlockExpectedSub(
                        initial: initial,
                        sub: BuildImmediateBytes(value, count),
                        count,
                        repeat);
                    byte[] actual = c.MEMC.RAM.GetMemoryAt(target.Resolve(c), count);
                    Assert.Equal(expected, actual);
                });
        }

        [Theory]
        [MemberData(nameof(MemoryRegisterCases))]
        public void SUB_memory_register(string name, AddressMode target, int widthBytes, Func<Computer, uint> sourceGetter, Action<Computer> sourceSetter)
        {
            uint initialValue = 0;
            RunAsmCase(
                name,
                $"SUB {target.Operand},{GetRegisterForWidth(widthBytes)}",
                c =>
                {
                    target.Prime(c);
                    PrepareMemorySpan(c, target.Resolve(c), (byte)widthBytes, Enumerable.Repeat((byte)10, widthBytes + 1).ToArray());
                    sourceSetter(c);
                    initialValue = ReadMemoryValue(c, target.Resolve(c), widthBytes);
                },
                c =>
                {
                    uint expected = SubMasked(initialValue, sourceGetter(c), widthBytes);
                    uint actual = ReadMemoryValue(c, target.Resolve(c), widthBytes);
                    Assert.Equal(expected, actual);
                });
        }

        [Theory]
        [MemberData(nameof(MemoryValueBlockCases))]
        public void SUB_memory_block_from_value(string name, AddressMode target, AddressMode valueMode, byte count, uint repeat)
        {
            byte[] initialTarget = [];
            byte[] valueBytes = [];

            RunAsmCase(
                name,
                $"SUB {target.Operand},{valueMode.Operand},{count},{TUtils.Get24bitRegisterString(RepeatRegIndex)}",
                c =>
                {
                    target.Prime(c);
                    valueMode.Prime(c);
                    byte[] targetSeed = new byte[] { 20, 30, 40, 50 };
                    byte[] valueSeed = new byte[] { 1, 2, 3, 4 };
                    PrepareMemorySpan(c, target.Resolve(c), count, targetSeed);
                    PrepareMemorySpan(c, valueMode.Resolve(c), count, valueSeed);
                    initialTarget = targetSeed.Take(count).ToArray();
                    valueBytes = valueSeed.Take(count).ToArray();
                    c.CPU.REGS.Set24BitRegister(RepeatRegIndex, repeat); // repeat register
                },
                c =>
                {
                    byte[] expected = ComputeBlockExpectedSub(
                        initial: initialTarget,
                        sub: valueBytes,
                        count,
                        repeat);
                    byte[] actual = c.MEMC.RAM.GetMemoryAt(target.Resolve(c), count);
                    Assert.Equal(expected, actual);
                });
        }

        [Theory]
        [MemberData(nameof(MemoryFloatCases))]
        public void SUB_memory_float(string name, AddressMode target)
        {
            RunAsmCase(
                name,
                $"SUB {target.Operand},F4",
                c =>
                {
                    target.Prime(c);
                    c.MEMC.SetFloatToRam(target.Resolve(c), 5.25f);
                    c.CPU.FREGS.SetRegister(4, 2.5f);
                },
                c =>
                {
                    float expected = 5.25f - 2.5f;
                    Assert.Equal(expected, c.MEMC.GetFloatFromRAM(target.Resolve(c)));
                });
        }

        [Theory]
        [MemberData(nameof(FloatRegisterCases))]
        public void SUB_fr_variations(string name, string asm, Action<Computer> arrange, Func<Computer, float> actual, float expected)
        {
            RunAsmCase(name, asm, arrange, c => Assert.Equal(expected, actual(c), precision: 5));
        }

        [Theory]
        [MemberData(nameof(FloatRegisterMemoryCases))]
        public void SUB_fr_with_all_addressing(string name, AddressMode source, byte destIndex)
        {
            RunAsmCase(
                name,
                $"SUB {TUtils.GetFloatRegisterString(destIndex)},{source.Operand}",
                c =>
                {
                    source.Prime(c);
                    c.CPU.FREGS.SetRegister(destIndex, 5.0f);
                    c.MEMC.SetFloatToRam(source.Resolve(c), 0.75f);
                },
                c =>
                {
                    float expected = 4.25f;
                    Assert.Equal(expected, c.CPU.FREGS.GetRegister(destIndex));
                });
        }

        #endregion

        #region Helpers

        private static object[] CreateRegisterCase(string name, string asm, Action<Computer> arrange, Func<Computer, uint> actual, uint expected)
        {
            return new object[] { name, asm, arrange, actual, expected };
        }

        private static object[] CreateFloatRegisterCase(string name, string asm, Action<Computer> arrange, Func<Computer, float> actual, float expected)
        {
            return new object[] { name, asm, arrange, actual, expected };
        }

        private static void RunAsmCase(string name, string asm, Action<Computer> arrange, Action<Computer> assertions)
        {
            Assembler cp = new();
            using Computer computer = new();

            arrange(computer);

            cp.Build(TUtils.GetFormattedAsm(asm, "BREAK"));
            if (cp.Errors > 0)
                throw new InvalidOperationException($"Assembly failed for '{name}': {cp.Log}");
            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            assertions(computer);
            TUtils.IncrementCountedTests("exec");
        }

        private static string FormatHex(uint value) => $"0x{value:X}";

        private static void PrepareMemorySpan(Computer computer, uint address, byte count, IReadOnlyList<byte> initial)
        {
            byte[] seed = new byte[count];
            for (int i = 0; i < count; i++)
                seed[i] = initial[i % initial.Count];

            computer.LoadMemAt(address, seed);
        }

        private static string GetRegisterForWidth(int widthBytes) =>
            widthBytes switch
            {
                1 => TUtils.Get8bitRegisterChar(1).ToString(),
                2 => TUtils.Get16bitRegisterString(2),
                3 => TUtils.Get24bitRegisterString(3),
                _ => TUtils.Get32bitRegisterString(4)
            };

        private static uint ReadMemoryValue(Computer computer, uint address, int widthBytes) =>
            widthBytes switch
            {
                1 => computer.MEMC.Get8bitFromRAM(address),
                2 => computer.MEMC.Get16bitFromRAM(address),
                3 => computer.MEMC.Get24bitFromRAM(address),
                _ => computer.MEMC.Get32bitFromRAM(address)
            };

        private static byte[] BuildImmediateBytes(uint value, byte count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                int idx = count - 1 - i;
                bytes[i] = idx >= 4 ? (byte)0 : (byte)(value >> (8 * idx));
            }

            return bytes;
        }

        private static byte[] ComputeBlockExpectedSub(byte[] initial, byte[] sub, byte count, uint repeat)
        {
            byte[] working = initial.Take(count).ToArray();
            if (repeat == 0)
                repeat = 1;

            for (uint r = 0; r < repeat; r++)
            {
                byte borrow = 0;
                for (int i = count - 1; i >= 0; i--)
                {
                    int diff = working[i] - sub[i] - borrow;
                    if (diff < 0)
                    {
                        diff += 256;
                        borrow = 1;
                    }
                    else
                    {
                        borrow = 0;
                    }
                    working[i] = (byte)diff;
                }
            }

            return working;
        }

        #endregion
    }

}
