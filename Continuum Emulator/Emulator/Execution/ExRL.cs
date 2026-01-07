using Continuum93.Emulator;
using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    /// <summary>
    /// RL execution unit (Rotate Left) built from ExSL.
    ///
    /// "Roll" semantics:
    /// - Bits that would be shifted out from the MSB side are wrapped around into the LSB side.
    /// - Rotation count is taken modulo the bit-width of the target (8/16/24/32 for scalars; countBytes*8 for blocks).
    ///
    /// Implements all 244 sub-op variations (0..243) via a 256-entry dispatch table.
    /// Any unassigned entries are no-ops (safety).
    ///
    /// Carry flag:
    /// - For scalar rotates: Carry is set to the last bit rotated out of the MSB side (i.e., the bit that wrapped).
    /// - For block rotates: Carry is set to the last bit rotated out of the MSB side (bit index rot-1 from MSB).
    /// </summary>
    public static class ExRL
    {
        private enum Width : byte
        {
            Byte = 1,
            Word = 2,
            TriByte = 3,
            DWord = 4
        }

        private static readonly Action<Computer>[] _dispatch = BuildDispatchTable();

        private static Action<Computer>[] BuildDispatchTable()
        {
            var table = new Action<Computer>[256];
            for (int i = 0; i < table.Length; i++)
                table[i] = _ => { };

            RegisterRegisterDestinations(table);
            RegisterMemoryGroups(table);
            RegisterFloatRegisterHandlers(table);

            return table;
        }

        // -------------------------
        // Register destinations
        // -------------------------
        private static void RegisterRegisterDestinations(Action<Computer>[] table)
        {
            // 8-bit destination
            table[Instructions._r_n] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                byte count = mem.Fetch();
                RotateRegisterLeft(cpu, Width.Byte, dest, count);
            };

            table[Instructions._r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.Byte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._r_InnnI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressAbs);
            table[Instructions._r_Innn_nnnI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressAbsOffsetImm);
            table[Instructions._r_Innn_rI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg8);
            table[Instructions._r_Innn_rrI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg16);
            table[Instructions._r_Innn_rrrI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg24);

            table[Instructions._r_IrrrI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressPtr);
            table[Instructions._r_Irrr_nnnI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressPtrOffsetImm);
            table[Instructions._r_Irrr_rI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg8);
            table[Instructions._r_Irrr_rrI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg16);
            table[Instructions._r_Irrr_rrrI] = cpu => RotateRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg24);

            table[Instructions._r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                RotateRegisterLeft(cpu, Width.Byte, dest, count);
            };

            // 16-bit destination
            table[Instructions._rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                ushort imm = mem.Fetch16();
                RotateRegisterLeft(cpu, Width.Word, dest, (byte)imm);
            };

            table[Instructions._rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.Word, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.Word, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rr_InnnI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressAbs);
            table[Instructions._rr_Innn_nnnI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressAbsOffsetImm);
            table[Instructions._rr_Innn_rI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg8);
            table[Instructions._rr_Innn_rrI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg16);
            table[Instructions._rr_Innn_rrrI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg24);

            table[Instructions._rr_IrrrI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressPtr);
            table[Instructions._rr_Irrr_nnnI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressPtrOffsetImm);
            table[Instructions._rr_Irrr_rI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg8);
            table[Instructions._rr_Irrr_rrI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg16);
            table[Instructions._rr_Irrr_rrrI] = cpu => RotateRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg24);

            table[Instructions._rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                RotateRegisterLeft(cpu, Width.Word, dest, count);
            };

            // 24-bit destination
            table[Instructions._rrr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm24 = mem.Fetch24();
                RotateRegisterLeft(cpu, Width.TriByte, dest, (byte)imm24);
            };

            table[Instructions._rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.TriByte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.TriByte, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.TriByte, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrr_InnnI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressAbs);
            table[Instructions._rrr_Innn_nnnI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetImm);
            table[Instructions._rrr_Innn_rI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg8);
            table[Instructions._rrr_Innn_rrI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg16);
            table[Instructions._rrr_Innn_rrrI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg24);

            table[Instructions._rrr_IrrrI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressPtr);
            table[Instructions._rrr_Irrr_nnnI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetImm);
            table[Instructions._rrr_Irrr_rI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg8);
            table[Instructions._rrr_Irrr_rrI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg16);
            table[Instructions._rrr_Irrr_rrrI] = cpu => RotateRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg24);

            table[Instructions._rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                RotateRegisterLeft(cpu, Width.TriByte, dest, count);
            };

            // 32-bit destination
            table[Instructions._rrrr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm32 = mem.Fetch32();
                RotateRegisterLeft(cpu, Width.DWord, dest, (byte)imm32);
            };

            table[Instructions._rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.DWord, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                RotateRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get32BitRegister(src));
            };

            table[Instructions._rrrr_InnnI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressAbs);
            table[Instructions._rrrr_Innn_nnnI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressAbsOffsetImm);
            table[Instructions._rrrr_Innn_rI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg8);
            table[Instructions._rrrr_Innn_rrI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg16);
            table[Instructions._rrrr_Innn_rrrI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg24);

            table[Instructions._rrrr_IrrrI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressPtr);
            table[Instructions._rrrr_Irrr_nnnI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressPtrOffsetImm);
            table[Instructions._rrrr_Irrr_rI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg8);
            table[Instructions._rrrr_Irrr_rrI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg16);
            table[Instructions._rrrr_Irrr_rrrI] = cpu => RotateRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg24);

            table[Instructions._rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                RotateRegisterLeft(cpu, Width.DWord, dest, count);
            };
        }

        private static void RotateRegFromMem8(Computer cpu, Width destWidth, AddressResolver resolver)
        {
            var mem = cpu.MEMC;
            var regs = cpu.CPU.REGS;
            byte dest = ReadRegIndex(mem);
            uint addr = resolver(mem, regs);
            byte count = mem.Get8bitFromRAM(addr);
            RotateRegisterLeft(cpu, destWidth, dest, count);
        }

        // -------------------------
        // Memory destinations + block groups
        // -------------------------
        private static void RegisterMemoryGroups(Action<Computer>[] table)
        {
            AddressResolver[] valueResolvers =
            [
                AddressAbs,
                AddressAbsOffsetImm,
                AddressAbsOffsetReg8,
                AddressAbsOffsetReg16,
                AddressAbsOffsetReg24,
                AddressPtr,
                AddressPtrOffsetImm,
                AddressPtrOffsetReg8,
                AddressPtrOffsetReg16,
                AddressPtrOffsetReg24
            ];

            TargetGroup[] groups =
            [
                new(Instructions._InnnI_nnnn_n, Instructions._InnnI_nnnn_n_nnn,
                    Instructions._InnnI_r, Instructions._InnnI_rr, Instructions._InnnI_rrr, Instructions._InnnI_rrrr,
                    [
                        Instructions._InnnI_InnnI_n_rrr, Instructions._InnnI_Innn_nnnI_n_rrr, Instructions._InnnI_Innn_rI_n_rrr,
                        Instructions._InnnI_Innn_rrI_n_rrr, Instructions._InnnI_Innn_rrrI_n_rrr, Instructions._InnnI_IrrrI_n_rrr,
                        Instructions._InnnI_Irrr_nnnI_n_rrr, Instructions._InnnI_Irrr_rI_n_rrr, Instructions._InnnI_Irrr_rrI_n_rrr,
                        Instructions._InnnI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._InnnI_fr,
                    AddressAbs),

                new(Instructions._Innn_nnnI_nnnn_n, Instructions._Innn_nnnI_nnnn_n_nnn,
                    Instructions._Innn_nnnI_r, Instructions._Innn_nnnI_rr, Instructions._Innn_nnnI_rrr, Instructions._Innn_nnnI_rrrr,
                    [
                        Instructions._Innn_nnnI_InnnI_n_rrr, Instructions._Innn_nnnI_Innn_nnnI_n_rrr, Instructions._Innn_nnnI_Innn_rI_n_rrr,
                        Instructions._Innn_nnnI_Innn_rrI_n_rrr, Instructions._Innn_nnnI_Innn_rrrI_n_rrr, Instructions._Innn_nnnI_IrrrI_n_rrr,
                        Instructions._Innn_nnnI_Irrr_nnnI_n_rrr, Instructions._Innn_nnnI_Irrr_rI_n_rrr, Instructions._Innn_nnnI_Irrr_rrI_n_rrr,
                        Instructions._Innn_nnnI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Innn_nnnI_fr,
                    AddressAbsOffsetImm),

                new(Instructions._Innn_rI_nnnn_n, Instructions._Innn_rI_nnnn_n_nnn,
                    Instructions._Innn_rI_r, Instructions._Innn_rI_rr, Instructions._Innn_rI_rrr, Instructions._Innn_rI_rrrr,
                    [
                        Instructions._Innn_rI_InnnI_n_rrr, Instructions._Innn_rI_Innn_nnnI_n_rrr, Instructions._Innn_rI_Innn_rI_n_rrr,
                        Instructions._Innn_rI_Innn_rrI_n_rrr, Instructions._Innn_rI_Innn_rrrI_n_rrr, Instructions._Innn_rI_IrrrI_n_rrr,
                        Instructions._Innn_rI_Irrr_nnnI_n_rrr, Instructions._Innn_rI_Irrr_rI_n_rrr, Instructions._Innn_rI_Irrr_rrI_n_rrr,
                        Instructions._Innn_rI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Innn_rI_fr,
                    AddressAbsOffsetReg8),

                new(Instructions._Innn_rrI_nnnn_n, Instructions._Innn_rrI_nnnn_n_nnn,
                    Instructions._Innn_rrI_r, Instructions._Innn_rrI_rr, Instructions._Innn_rrI_rrr, Instructions._Innn_rrI_rrrr,
                    [
                        Instructions._Innn_rrI_InnnI_n_rrr, Instructions._Innn_rrI_Innn_nnnI_n_rrr, Instructions._Innn_rrI_Innn_rI_n_rrr,
                        Instructions._Innn_rrI_Innn_rrI_n_rrr, Instructions._Innn_rrI_Innn_rrrI_n_rrr, Instructions._Innn_rrI_IrrrI_n_rrr,
                        Instructions._Innn_rrI_Irrr_nnnI_n_rrr, Instructions._Innn_rrI_Irrr_rI_n_rrr, Instructions._Innn_rrI_Irrr_rrI_n_rrr,
                        Instructions._Innn_rrI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Innn_rrI_fr,
                    AddressAbsOffsetReg16),

                new(Instructions._Innn_rrrI_nnnn_n, Instructions._Innn_rrrI_nnnn_n_nnn,
                    Instructions._Innn_rrrI_r, Instructions._Innn_rrrI_rr, Instructions._Innn_rrrI_rrr, Instructions._Innn_rrrI_rrrr,
                    [
                        Instructions._Innn_rrrI_InnnI_n_rrr, Instructions._Innn_rrrI_Innn_nnnI_n_rrr, Instructions._Innn_rrrI_Innn_rI_n_rrr,
                        Instructions._Innn_rrrI_Innn_rrI_n_rrr, Instructions._Innn_rrrI_Innn_rrrI_n_rrr, Instructions._Innn_rrrI_IrrrI_n_rrr,
                        Instructions._Innn_rrrI_Irrr_nnnI_n_rrr, Instructions._Innn_rrrI_Irrr_rI_n_rrr, Instructions._Innn_rrrI_Irrr_rrI_n_rrr,
                        Instructions._Innn_rrrI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Innn_rrrI_fr,
                    AddressAbsOffsetReg24),

                new(Instructions._IrrrI_nnnn_n, Instructions._IrrrI_nnnn_n_nnn,
                    Instructions._IrrrI_r, Instructions._IrrrI_rr, Instructions._IrrrI_rrr, Instructions._IrrrI_rrrr,
                    [
                        Instructions._IrrrI_InnnI_n_rrr, Instructions._IrrrI_Innn_nnnI_n_rrr, Instructions._IrrrI_Innn_rI_n_rrr,
                        Instructions._IrrrI_Innn_rrI_n_rrr, Instructions._IrrrI_Innn_rrrI_n_rrr, Instructions._IrrrI_IrrrI_n_rrr,
                        Instructions._IrrrI_Irrr_nnnI_n_rrr, Instructions._IrrrI_Irrr_rI_n_rrr, Instructions._IrrrI_Irrr_rrI_n_rrr,
                        Instructions._IrrrI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._IrrrI_fr,
                    AddressPtr),

                new(Instructions._Irrr_nnnI_nnnn_n, Instructions._Irrr_nnnI_nnnn_n_nnn,
                    Instructions._Irrr_nnnI_r, Instructions._Irrr_nnnI_rr, Instructions._Irrr_nnnI_rrr, Instructions._Irrr_nnnI_rrrr,
                    [
                        Instructions._Irrr_nnnI_InnnI_n_rrr, Instructions._Irrr_nnnI_Innn_nnnI_n_rrr, Instructions._Irrr_nnnI_Innn_rI_n_rrr,
                        Instructions._Irrr_nnnI_Innn_rrI_n_rrr, Instructions._Irrr_nnnI_Innn_rrrI_n_rrr, Instructions._Irrr_nnnI_IrrrI_n_rrr,
                        Instructions._Irrr_nnnI_Irrr_nnnI_n_rrr, Instructions._Irrr_nnnI_Irrr_rI_n_rrr, Instructions._Irrr_nnnI_Irrr_rrI_n_rrr,
                        Instructions._Irrr_nnnI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Irrr_nnnI_fr,
                    AddressPtrOffsetImm),

                new(Instructions._Irrr_rI_nnnn_n, Instructions._Irrr_rI_nnnn_n_nnn,
                    Instructions._Irrr_rI_r, Instructions._Irrr_rI_rr, Instructions._Irrr_rI_rrr, Instructions._Irrr_rI_rrrr,
                    [
                        Instructions._Irrr_rI_InnnI_n_rrr, Instructions._Irrr_rI_Innn_nnnI_n_rrr, Instructions._Irrr_rI_Innn_rI_n_rrr,
                        Instructions._Irrr_rI_Innn_rrI_n_rrr, Instructions._Irrr_rI_Innn_rrrI_n_rrr, Instructions._Irrr_rI_IrrrI_n_rrr,
                        Instructions._Irrr_rI_Irrr_nnnI_n_rrr, Instructions._Irrr_rI_Irrr_rI_n_rrr, Instructions._Irrr_rI_Irrr_rrI_n_rrr,
                        Instructions._Irrr_rI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Irrr_rI_fr,
                    AddressPtrOffsetReg8),

                new(Instructions._Irrr_rrI_nnnn_n, Instructions._Irrr_rrI_nnnn_n_nnn,
                    Instructions._Irrr_rrI_r, Instructions._Irrr_rrI_rr, Instructions._Irrr_rrI_rrr, Instructions._Irrr_rrI_rrrr,
                    [
                        Instructions._Irrr_rrI_InnnI_n_rrr, Instructions._Irrr_rrI_Innn_nnnI_n_rrr, Instructions._Irrr_rrI_Innn_rI_n_rrr,
                        Instructions._Irrr_rrI_Innn_rrI_n_rrr, Instructions._Irrr_rrI_Innn_rrrI_n_rrr, Instructions._Irrr_rrI_IrrrI_n_rrr,
                        Instructions._Irrr_rrI_Irrr_nnnI_n_rrr, Instructions._Irrr_rrI_Irrr_rI_n_rrr, Instructions._Irrr_rrI_Irrr_rrI_n_rrr,
                        Instructions._Irrr_rrI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Irrr_rrI_fr,
                    AddressPtrOffsetReg16),

                new(Instructions._Irrr_rrrI_nnnn_n, Instructions._Irrr_rrrI_nnnn_n_nnn,
                    Instructions._Irrr_rrrI_r, Instructions._Irrr_rrrI_rr, Instructions._Irrr_rrrI_rrr, Instructions._Irrr_rrrI_rrrr,
                    [
                        Instructions._Irrr_rrrI_InnnI_n_rrr, Instructions._Irrr_rrrI_Innn_nnnI_n_rrr, Instructions._Irrr_rrrI_Innn_rI_n_rrr,
                        Instructions._Irrr_rrrI_Innn_rrI_n_rrr, Instructions._Irrr_rrrI_Innn_rrrI_n_rrr, Instructions._Irrr_rrrI_IrrrI_n_rrr,
                        Instructions._Irrr_rrrI_Irrr_nnnI_n_rrr, Instructions._Irrr_rrrI_Irrr_rI_n_rrr, Instructions._Irrr_rrrI_Irrr_rrI_n_rrr,
                        Instructions._Irrr_rrrI_Irrr_rrrI_n_rrr
                    ],
                    Instructions._Irrr_rrrI_fr,
                    AddressPtrOffsetReg24)
            ];

            foreach (var group in groups)
            {
                // Block immediate rotate (count bytes, rotCount=Fetch32, count=Fetch, repeat=1)
                table[group.BlockImmediateOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint rotCount = mem.Fetch32();
                    byte count = mem.Fetch();
                    return (target, rotCount, count, 1u, false);
                });

                // Block immediate rotate with repeat (repeat=Fetch24)
                table[group.BlockImmediateRepeatOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint rotCount = mem.Fetch32();
                    byte count = mem.Fetch();
                    uint repeat = mem.Fetch24();
                    return (target, rotCount, count, repeat, false);
                });

                // Register->memory rotates (memory value rotated by register value)
                table[group.ByteRegOpcode] = MemHandler(Width.Byte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get8BitRegister(ReadRegIndex(mem))));
                table[group.WordRegOpcode] = MemHandler(Width.Word, (mem, regs) => (group.AddressResolver(mem, regs), (uint)regs.Get16BitRegister(ReadRegIndex(mem))));
                table[group.TriRegOpcode] = MemHandler(Width.TriByte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get24BitRegister(ReadRegIndex(mem))));
                table[group.DWordRegOpcode] = MemHandler(Width.DWord, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get32BitRegister(ReadRegIndex(mem))));

                // Memory-sourced block rotates with repeat in rrr (same shape as ExSL)
                for (int i = 0; i < valueResolvers.Length; i++)
                {
                    byte opcode = group.ValueOpcodeMatrix[i];
                    AddressResolver valueResolver = valueResolvers[i];

                    table[opcode] = BlockHandler((mem, regs) =>
                    {
                        uint target = group.AddressResolver(mem, regs);
                        uint valueAddress = valueResolver(mem, regs);
                        byte count = mem.Fetch();
                        uint repeat = regs.Get24BitRegister(ReadRegIndex(mem));
                        return (target, valueAddress, count, repeat, true);
                    });
                }

                // Float->memory rotates: treat memory float as bits, rotate by float-reg-derived count
                table[group.FloatOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;
                    var fregs = cpu.CPU.FREGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte fIndex = ReadFloatRegIndex(mem);

                    float current = mem.GetFloatFromRAM(address);
                    uint bits = FloatPointUtils.FloatToUint(current);

                    byte count = FloatToCountByte(fregs.GetRegister(fIndex));
                    bool carry;
                    bits = RotateLeftU32(bits, count, out carry);
                    cpu.CPU.FLAGS.SetCarry(carry);

                    mem.SetFloatToRam(address, FloatPointUtils.UintToFloat(bits));
                };
            }
        }

        // -------------------------
        // Float register destinations (_fr_*)
        // -------------------------
        private static void RegisterFloatRegisterHandlers(Action<Computer>[] table)
        {
            // RL fr, fr  (bitwise on IEEE754 bits)
            table[Instructions._fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                byte count = FloatToCountByte(fregs.GetRegister(src));

                bool carry;
                bits = RotateLeftU32(bits, count, out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RL fr, nnnn  (count from 32-bit immediate; low byte used)
            table[Instructions._fr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint rotCount = mem.Fetch32();

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, (byte)(rotCount & 0xFF), out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RL fr, r / rr / rrr / rrrr  (count from integer regs; low byte used)
            table[Instructions._fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, regs.Get8BitRegister(src), out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            table[Instructions._fr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, (byte)regs.Get16BitRegister(src), out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            table[Instructions._fr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, (byte)regs.Get24BitRegister(src), out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            table[Instructions._fr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, (byte)regs.Get32BitRegister(src), out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RL fr, (mem8) via the shared address resolvers (count read as 8-bit from memory)
            table[Instructions._fr_InnnI] = FloatRegMem8CountHandler(AddressAbs);
            table[Instructions._fr_Innn_nnnI] = FloatRegMem8CountHandler(AddressAbsOffsetImm);
            table[Instructions._fr_Innn_rI] = FloatRegMem8CountHandler(AddressAbsOffsetReg8);
            table[Instructions._fr_Innn_rrI] = FloatRegMem8CountHandler(AddressAbsOffsetReg16);
            table[Instructions._fr_Innn_rrrI] = FloatRegMem8CountHandler(AddressAbsOffsetReg24);

            table[Instructions._fr_IrrrI] = FloatRegMem8CountHandler(AddressPtr);
            table[Instructions._fr_Irrr_nnnI] = FloatRegMem8CountHandler(AddressPtrOffsetImm);
            table[Instructions._fr_Irrr_rI] = FloatRegMem8CountHandler(AddressPtrOffsetReg8);
            table[Instructions._fr_Irrr_rrI] = FloatRegMem8CountHandler(AddressPtrOffsetReg16);
            table[Instructions._fr_Irrr_rrrI] = FloatRegMem8CountHandler(AddressPtrOffsetReg24);
        }

        private static Action<Computer> FloatRegMem8CountHandler(AddressResolver resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint addr = resolver(mem, regs);
                byte count = mem.Get8bitFromRAM(addr);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));

                bool carry;
                bits = RotateLeftU32(bits, count, out carry);
                cpu.CPU.FLAGS.SetCarry(carry);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };
        }

        // -------------------------
        // Helpers: memory/block handlers
        // -------------------------
        private sealed record TargetGroup(
            byte BlockImmediateOpcode,
            byte BlockImmediateRepeatOpcode,
            byte ByteRegOpcode,
            byte WordRegOpcode,
            byte TriRegOpcode,
            byte DWordRegOpcode,
            byte[] ValueOpcodeMatrix,
            byte FloatOpcode,
            AddressResolver AddressResolver);

        private delegate uint AddressResolver(MemoryController mem, Registers regs);

        private static uint AddressAbs(MemoryController mem, Registers regs) => mem.Fetch24();
        private static uint AddressAbsOffsetImm(MemoryController mem, Registers regs) => OffsetAddress(mem.Fetch24(), mem.Fetch24Signed());
        private static uint AddressAbsOffsetReg8(MemoryController mem, Registers regs) => OffsetAddress(mem.Fetch24(), regs.Get8BitRegisterSigned(ReadRegIndex(mem)));
        private static uint AddressAbsOffsetReg16(MemoryController mem, Registers regs) => OffsetAddress(mem.Fetch24(), regs.Get16BitRegisterSigned(ReadRegIndex(mem)));
        private static uint AddressAbsOffsetReg24(MemoryController mem, Registers regs) => OffsetAddress(mem.Fetch24(), regs.Get24BitRegisterSigned(ReadRegIndex(mem)));

        private static uint AddressPtr(MemoryController mem, Registers regs) => regs.Get24BitRegister(ReadRegIndex(mem));
        private static uint AddressPtrOffsetImm(MemoryController mem, Registers regs) => OffsetAddress(regs.Get24BitRegister(ReadRegIndex(mem)), mem.Fetch24Signed());
        private static uint AddressPtrOffsetReg8(MemoryController mem, Registers regs) => OffsetAddress(regs.Get24BitRegister(ReadRegIndex(mem)), regs.Get8BitRegisterSigned(ReadRegIndex(mem)));
        private static uint AddressPtrOffsetReg16(MemoryController mem, Registers regs) => OffsetAddress(regs.Get24BitRegister(ReadRegIndex(mem)), regs.Get16BitRegisterSigned(ReadRegIndex(mem)));
        private static uint AddressPtrOffsetReg24(MemoryController mem, Registers regs) => OffsetAddress(regs.Get24BitRegister(ReadRegIndex(mem)), regs.Get24BitRegisterSigned(ReadRegIndex(mem)));

        private static Action<Computer> MemHandler(Width width, Func<MemoryController, Registers, (uint address, uint rotCount)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, rotCount) = resolver(mem, regs);
                RotateMemoryValue(cpu, width, address, (byte)(rotCount & 0xFF));
            };
        }

        private static Action<Computer> BlockHandler(Func<MemoryController, Registers, (uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, valueOrAddress, count, repeat, sourceIsAddress) = resolver(mem, regs);
                RotateBlock(cpu, address, valueOrAddress, count, repeat, sourceIsAddress);
            };
        }

        private static void RotateMemoryValue(Computer cpu, Width width, uint address, byte rawCount)
        {
            var mem = cpu.MEMC;

            int bits = width == Width.Byte ? 8 :
                       width == Width.Word ? 16 :
                       width == Width.TriByte ? 24 : 32;

            byte count = (byte)(rawCount % bits);
            if (count == 0)
            {
                cpu.CPU.FLAGS.SetCarry(false);
                return;
            }

            bool carry;

            switch (width)
            {
                case Width.Byte:
                    {
                        byte v = mem.Get8bitFromRAM(address);
                        byte r = RotateLeftU8(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        mem.Set8bitToRAM(address, r);
                        break;
                    }
                case Width.Word:
                    {
                        ushort v = mem.Get16bitFromRAM(address);
                        ushort r = RotateLeftU16(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        mem.Set16bitToRAM(address, r);
                        break;
                    }
                case Width.TriByte:
                    {
                        uint v = mem.Get24bitFromRAM(address) & 0xFFFFFFu;
                        uint r = RotateLeftU24(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        mem.Set24bitToRAM(address, r);
                        break;
                    }
                case Width.DWord:
                    {
                        uint v = mem.Get32bitFromRAM(address);
                        uint r = RotateLeftU32(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        mem.Set32bitToRAM(address, r);
                        break;
                    }
            }
        }

        private static void RotateBlock(Computer cpu, uint address, uint valueOrAddress, byte countBytes, uint repeat, bool sourceIsAddress)
        {
            var mem = cpu.MEMC;

            if (countBytes == 0) countBytes = 1;
            if (repeat == 0) repeat = 1;

            uint bitWidth = (uint)countBytes * 8u;

            uint rotCountBits;

            if (!sourceIsAddress)
            {
                // immediate 32-bit bit-count (as-is)
                rotCountBits = valueOrAddress;
            }
            else
            {
                // REQUIRED SEMANTICS (carried over from ExSL): rotation count is a SINGLE BYTE at valueOrAddress
                rotCountBits = mem.Get8bitFromRAM(valueOrAddress);
            }

            // REQUIRED SEMANTICS: apply it "repeat" times -> total bits rotated = rotCountBits * repeat
            uint totalRotBits = rotCountBits * repeat;

            if (bitWidth == 0)
            {
                cpu.CPU.FLAGS.SetCarry(false);
                return;
            }

            uint rot = totalRotBits % bitWidth;
            if (rot == 0)
            {
                cpu.CPU.FLAGS.SetCarry(false);
                return;
            }

            // Carry = last bit rotated out of the MSB side: bit index (rot-1) from MSB.
            bool carry = GetBitBigEndian(mem, address, countBytes, (int)(rot - 1));
            cpu.CPU.FLAGS.SetCarry(carry);

            RotateLeftBigEndianInPlace(mem, address, countBytes, (int)rot);
        }

        private static void RotateRegisterLeft(Computer cpu, Width width, byte regIndex, byte rawCount)
        {
            var regs = cpu.CPU.REGS;

            int bits = width == Width.Byte ? 8 :
                       width == Width.Word ? 16 :
                       width == Width.TriByte ? 24 : 32;

            byte count = (byte)(rawCount % bits);
            if (count == 0)
            {
                cpu.CPU.FLAGS.SetCarry(false);
                return;
            }

            bool carry;

            switch (width)
            {
                case Width.Byte:
                    {
                        byte v = regs.Get8BitRegister(regIndex);
                        byte r = RotateLeftU8(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        regs.Set8BitRegister(regIndex, r);
                        break;
                    }
                case Width.Word:
                    {
                        ushort v = regs.Get16BitRegister(regIndex);
                        ushort r = RotateLeftU16(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        regs.Set16BitRegister(regIndex, r);
                        break;
                    }
                case Width.TriByte:
                    {
                        uint v = regs.Get24BitRegister(regIndex) & 0xFFFFFFu;
                        uint r = RotateLeftU24(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        regs.Set24BitRegister(regIndex, r);
                        break;
                    }
                case Width.DWord:
                    {
                        uint v = regs.Get32BitRegister(regIndex);
                        uint r = RotateLeftU32(v, count, out carry);
                        cpu.CPU.FLAGS.SetCarry(carry);
                        regs.Set32BitRegister(regIndex, r);
                        break;
                    }
            }
        }

        private static uint OffsetAddress(uint baseAddr, int offset) => (uint)(baseAddr + offset);

        private static byte FloatToCountByte(float f)
        {
            f = Math.Abs(f);
            if (float.IsNaN(f) || float.IsInfinity(f)) return 0;
            int n = (int)Math.Round(f);
            if (n < 0) return 0;
            if (n > 255) return 255;
            return (byte)n;
        }

        private static byte ReadRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x1F);
        private static byte ReadFloatRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x0F);

        // -------------------------
        // Scalar rotate helpers + carry
        // -------------------------
        private static byte RotateLeftU8(byte v, int count, out bool carry)
        {
            count &= 7;
            if (count == 0) { carry = false; return v; }
            carry = ((v >> (8 - count)) & 0x1) != 0;
            return (byte)((v << count) | (v >> (8 - count)));
        }

        private static ushort RotateLeftU16(ushort v, int count, out bool carry)
        {
            count &= 15;
            if (count == 0) { carry = false; return v; }
            carry = ((v >> (16 - count)) & 0x1) != 0;
            return (ushort)((v << count) | (v >> (16 - count)));
        }

        private static uint RotateLeftU24(uint v, int count, out bool carry)
        {
            count %= 24;
            if (count == 0) { carry = false; return v & 0xFFFFFFu; }
            v &= 0xFFFFFFu;
            carry = ((v >> (24 - count)) & 0x1) != 0;
            return ((v << count) | (v >> (24 - count))) & 0xFFFFFFu;
        }

        private static uint RotateLeftU32(uint v, byte rawCount, out bool carry)
        {
            int count = rawCount & 31; // 0..31
            if (count == 0) { carry = false; return v; }
            carry = ((v >> (32 - count)) & 0x1u) != 0;
            return (v << count) | (v >> (32 - count));
        }

        // -------------------------
        // Big-endian block rotate helpers
        // -------------------------
        private static bool GetBitBigEndian(MemoryController mem, uint baseAddr, byte countBytes, int bitIndexFromMsb)
        {
            int byteIndex = bitIndexFromMsb / 8;
            int bitInByte = bitIndexFromMsb % 8; // 0 = MSB
            byte b = mem.Get8bitFromRAM(baseAddr + (uint)byteIndex);
            int mask = 1 << (7 - bitInByte);
            return (b & mask) != 0;
        }

        private static void SetBitBigEndian(byte[] buf, int bitIndexFromMsb, bool value)
        {
            int byteIndex = bitIndexFromMsb / 8;
            int bitInByte = bitIndexFromMsb % 8;
            int mask = 1 << (7 - bitInByte);
            if (value) buf[byteIndex] = (byte)(buf[byteIndex] | mask);
            else buf[byteIndex] = (byte)(buf[byteIndex] & ~mask);
        }

        private static bool GetBitBigEndian(byte[] buf, int bitIndexFromMsb)
        {
            int byteIndex = bitIndexFromMsb / 8;
            int bitInByte = bitIndexFromMsb % 8;
            int mask = 1 << (7 - bitInByte);
            return (buf[byteIndex] & mask) != 0;
        }

        private static void RotateLeftBigEndianInPlace(MemoryController mem, uint baseAddr, byte countBytes, int rotBits)
        {
            int bitWidth = countBytes * 8;
            rotBits %= bitWidth;
            if (rotBits == 0) return;

            byte[] src = new byte[countBytes];
            for (int i = 0; i < countBytes; i++)
                src[i] = mem.Get8bitFromRAM(baseAddr + (uint)i);

            byte[] dst = new byte[countBytes];

            // dest[p] = src[(p + rotBits) % bitWidth]   (p is MSB-indexed)
            for (int p = 0; p < bitWidth; p++)
            {
                int s = p + rotBits;
                if (s >= bitWidth) s -= bitWidth;
                bool bit = GetBitBigEndian(src, s);
                SetBitBigEndian(dst, p, bit);
            }

            for (int i = 0; i < countBytes; i++)
                mem.Set8bitToRAM(baseAddr + (uint)i, dst[i]);
        }

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
