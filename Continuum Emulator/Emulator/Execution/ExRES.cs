using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    /// <summary>
    /// RES execution unit updated to use the shared Instructions sub-opcodes
    /// (the same addressing matrix used by LD / ADD / SUB / DIV / MUL / SL / SR ...).
    ///
    /// Implements all 244 sub-op variations (0..243) via a 256-entry dispatch table.
    /// Any unassigned entries are no-ops (safety).
    ///
    /// Semantics implemented here (bit-reset):
    /// - Register destinations: reset (to 0) the bit indexed by a bit-position resolved from
    ///   immediate / register / memory / float-reg (per sub-op), in the destination register.
    /// - Memory destinations: reset (to 0) the bit indexed by the resolved bit-position in the
    ///   value stored at the resolved address (width per sub-op).
    /// - Block destinations:
    ///     * Immediate bit index: set one bit inside a big-endian unsigned integer block of length "count" bytes,
    ///       where bitIndex is a 32-bit immediate. With "repeat" (rrr) we set a run of bits starting at bitIndex.
    ///     * Memory-sourced bit index: read ONE BYTE at valueAddress as the bitIndex, multiply by repeat (rrr),
    ///       and set the resulting bit inside the target block.
    /// - Float registers (_fr_*): treated as bitwise operation on the IEEE754 payload (uint bits),
    ///   setting a bit and returning a float from the modified bits.
    /// </summary>
    public static class ExRES
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
                byte bitIndex = mem.Fetch();
                ResetRegisterBit(cpu, Width.Byte, dest, bitIndex);
            };

            table[Instructions._r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.Byte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._r_InnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressAbs);
            table[Instructions._r_Innn_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressAbsOffsetImm);
            table[Instructions._r_Innn_rI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressAbsOffsetReg8);
            table[Instructions._r_Innn_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressAbsOffsetReg16);
            table[Instructions._r_Innn_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressAbsOffsetReg24);

            table[Instructions._r_IrrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressPtr);
            table[Instructions._r_Irrr_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressPtrOffsetImm);
            table[Instructions._r_Irrr_rI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressPtrOffsetReg8);
            table[Instructions._r_Irrr_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressPtrOffsetReg16);
            table[Instructions._r_Irrr_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Byte, AddressPtrOffsetReg24);

            table[Instructions._r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte bitIndex = FloatToIndexByte(fregs.GetRegister(fIdx));
                ResetRegisterBit(cpu, Width.Byte, dest, bitIndex);
            };

            // 16-bit destination
            table[Instructions._rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                ushort imm = mem.Fetch16();
                ResetRegisterBit(cpu, Width.Word, dest, (byte)imm);
            };

            table[Instructions._rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.Word, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.Word, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rr_InnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressAbs);
            table[Instructions._rr_Innn_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressAbsOffsetImm);
            table[Instructions._rr_Innn_rI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressAbsOffsetReg8);
            table[Instructions._rr_Innn_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressAbsOffsetReg16);
            table[Instructions._rr_Innn_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressAbsOffsetReg24);

            table[Instructions._rr_IrrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressPtr);
            table[Instructions._rr_Irrr_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressPtrOffsetImm);
            table[Instructions._rr_Irrr_rI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressPtrOffsetReg8);
            table[Instructions._rr_Irrr_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressPtrOffsetReg16);
            table[Instructions._rr_Irrr_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.Word, AddressPtrOffsetReg24);

            table[Instructions._rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte bitIndex = FloatToIndexByte(fregs.GetRegister(fIdx));
                ResetRegisterBit(cpu, Width.Word, dest, bitIndex);
            };

            // 24-bit destination
            table[Instructions._rrr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm24 = mem.Fetch24();
                ResetRegisterBit(cpu, Width.TriByte, dest, (byte)imm24);
            };

            table[Instructions._rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.TriByte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.TriByte, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.TriByte, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrr_InnnI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressAbs);
            table[Instructions._rrr_Innn_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressAbsOffsetImm);
            table[Instructions._rrr_Innn_rI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg8);
            table[Instructions._rrr_Innn_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg16);
            table[Instructions._rrr_Innn_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg24);

            table[Instructions._rrr_IrrrI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressPtr);
            table[Instructions._rrr_Irrr_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressPtrOffsetImm);
            table[Instructions._rrr_Irrr_rI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg8);
            table[Instructions._rrr_Irrr_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg16);
            table[Instructions._rrr_Irrr_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg24);

            table[Instructions._rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte bitIndex = FloatToIndexByte(fregs.GetRegister(fIdx));
                ResetRegisterBit(cpu, Width.TriByte, dest, bitIndex);
            };

            // 32-bit destination
            table[Instructions._rrrr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm32 = mem.Fetch32();
                ResetRegisterBit(cpu, Width.DWord, dest, (byte)imm32);
            };

            table[Instructions._rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.DWord, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.DWord, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.DWord, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ResetRegisterBit(cpu, Width.DWord, dest, (byte)regs.Get32BitRegister(src));
            };

            table[Instructions._rrrr_InnnI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressAbs);
            table[Instructions._rrrr_Innn_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressAbsOffsetImm);
            table[Instructions._rrrr_Innn_rI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressAbsOffsetReg8);
            table[Instructions._rrrr_Innn_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressAbsOffsetReg16);
            table[Instructions._rrrr_Innn_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressAbsOffsetReg24);

            table[Instructions._rrrr_IrrrI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressPtr);
            table[Instructions._rrrr_Irrr_nnnI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressPtrOffsetImm);
            table[Instructions._rrrr_Irrr_rI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressPtrOffsetReg8);
            table[Instructions._rrrr_Irrr_rrI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressPtrOffsetReg16);
            table[Instructions._rrrr_Irrr_rrrI] = cpu => ResetRegBitFromMem8(cpu, Width.DWord, AddressPtrOffsetReg24);

            table[Instructions._rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte bitIndex = FloatToIndexByte(fregs.GetRegister(fIdx));
                ResetRegisterBit(cpu, Width.DWord, dest, bitIndex);
            };
        }

        private static void ResetRegBitFromMem8(Computer cpu, Width destWidth, AddressResolver resolver)
        {
            var mem = cpu.MEMC;
            var regs = cpu.CPU.REGS;
            byte dest = ReadRegIndex(mem);
            uint addr = resolver(mem, regs);
            byte bitIndex = mem.Get8bitFromRAM(addr);
            ResetRegisterBit(cpu, destWidth, dest, bitIndex);
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
                // Block: bitIndex from immediate 32-bit, count bytes, repeat=1
                table[group.BlockImmediateOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint bitIndex = mem.Fetch32();
                    byte count = mem.Fetch();
                    return (target, bitIndex, count, 1u, false);
                });

                // Block: bitIndex from immediate 32-bit, repeat from imm24
                table[group.BlockImmediateRepeatOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint bitIndex = mem.Fetch32();
                    byte count = mem.Fetch();
                    uint repeat = mem.Fetch24();
                    return (target, bitIndex, count, repeat, false);
                });

                // Register->memory bit-reset (bit index from register; low byte used)
                table[group.ByteRegOpcode] = MemHandler(Width.Byte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get8BitRegister(ReadRegIndex(mem))));
                table[group.WordRegOpcode] = MemHandler(Width.Word, (mem, regs) => (group.AddressResolver(mem, regs), (uint)regs.Get16BitRegister(ReadRegIndex(mem))));
                table[group.TriRegOpcode] = MemHandler(Width.TriByte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get24BitRegister(ReadRegIndex(mem))));
                table[group.DWordRegOpcode] = MemHandler(Width.DWord, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get32BitRegister(ReadRegIndex(mem))));

                // Memory-sourced block bit index with repeat in rrr (same shape as ExSL/ExSR groups)
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

                // Float->memory bit-reset: treat memory float as bits, set bit by float-reg-derived index
                table[group.FloatOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;
                    var fregs = cpu.CPU.FREGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte fIndex = ReadFloatRegIndex(mem);

                    float current = mem.GetFloatFromRAM(address);
                    uint bits = FloatPointUtils.FloatToUint(current);

                    byte bitIndex = FloatToIndexByte(fregs.GetRegister(fIndex));
                    bits = ResetBitU32(bits, bitIndex);

                    mem.SetFloatToRam(address, FloatPointUtils.UintToFloat(bits));
                };
            }
        }

        // -------------------------
        // Float register destinations (_fr_*): bitwise on IEEE754 bits
        // -------------------------
        private static void RegisterFloatRegisterHandlers(Action<Computer>[] table)
        {
            // RES fr, fr  (bit index from src float; modifies dest float bits)
            table[Instructions._fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                byte bitIndex = FloatToIndexByte(fregs.GetRegister(src));
                bits = ResetBitU32(bits, bitIndex);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RES fr, nnnn  (bit index from 32-bit immediate; low byte used)
            table[Instructions._fr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint bitIndex = mem.Fetch32();

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                bits = ResetBitU32(bits, (byte)(bitIndex & 0xFF));
                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RES fr, r / rr / rrr / rrrr  (bit index from integer regs; low byte used)
            table[Instructions._fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                bits = ResetBitU32(bits, regs.Get8BitRegister(src));
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
                bits = ResetBitU32(bits, (byte)regs.Get16BitRegister(src));
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
                bits = ResetBitU32(bits, (byte)regs.Get24BitRegister(src));
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
                bits = ResetBitU32(bits, (byte)regs.Get32BitRegister(src));
                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // RES fr, (mem8) via shared address resolvers (bit index read as 8-bit from memory)
            table[Instructions._fr_InnnI] = FloatRegMem8IndexHandler(AddressAbs);
            table[Instructions._fr_Innn_nnnI] = FloatRegMem8IndexHandler(AddressAbsOffsetImm);
            table[Instructions._fr_Innn_rI] = FloatRegMem8IndexHandler(AddressAbsOffsetReg8);
            table[Instructions._fr_Innn_rrI] = FloatRegMem8IndexHandler(AddressAbsOffsetReg16);
            table[Instructions._fr_Innn_rrrI] = FloatRegMem8IndexHandler(AddressAbsOffsetReg24);

            table[Instructions._fr_IrrrI] = FloatRegMem8IndexHandler(AddressPtr);
            table[Instructions._fr_Irrr_nnnI] = FloatRegMem8IndexHandler(AddressPtrOffsetImm);
            table[Instructions._fr_Irrr_rI] = FloatRegMem8IndexHandler(AddressPtrOffsetReg8);
            table[Instructions._fr_Irrr_rrI] = FloatRegMem8IndexHandler(AddressPtrOffsetReg16);
            table[Instructions._fr_Irrr_rrrI] = FloatRegMem8IndexHandler(AddressPtrOffsetReg24);
        }

        private static Action<Computer> FloatRegMem8IndexHandler(AddressResolver resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint addr = resolver(mem, regs);
                byte bitIndex = mem.Get8bitFromRAM(addr);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                bits = ResetBitU32(bits, bitIndex);
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

        private static Action<Computer> MemHandler(Width width, Func<MemoryController, Registers, (uint address, uint bitIndex)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, bitIndex) = resolver(mem, regs);
                ResetMemoryBit(cpu, width, address, (byte)(bitIndex & 0xFF));
            };
        }

        private static Action<Computer> BlockHandler(Func<MemoryController, Registers, (uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, valueOrAddress, count, repeat, sourceIsAddress) = resolver(mem, regs);
                ResetBlockBit(cpu, address, valueOrAddress, count, repeat, sourceIsAddress);
            };
        }

        private static void ResetMemoryBit(Computer cpu, Width width, uint address, byte rawBitIndex)
        {
            var mem = cpu.MEMC;
            int bits = width == Width.Byte ? 8 :
                       width == Width.Word ? 16 :
                       width == Width.TriByte ? 24 : 32;

            int bitIndex = rawBitIndex % bits;

            switch (width)
            {
                case Width.Byte:
                    {
                        byte v = mem.Get8bitFromRAM(address);
                        v = (byte)(v & (byte)~(1u << bitIndex));
                        mem.Set8bitToRAM(address, v);
                        break;
                    }
                case Width.Word:
                    {
                        ushort v = mem.Get16bitFromRAM(address);
                        v = (ushort)(v & (ushort)~(1u << bitIndex));
                        mem.Set16bitToRAM(address, v);
                        break;
                    }
                case Width.TriByte:
                    {
                        uint v = mem.Get24bitFromRAM(address) & 0xFFFFFFu;
                        v &= ~(1u << bitIndex);
                        mem.Set24bitToRAM(address, v & 0xFFFFFFu);
                        break;
                    }
                case Width.DWord:
                    {
                        uint v = mem.Get32bitFromRAM(address);
                        v &= ~(1u << bitIndex);
                        mem.Set32bitToRAM(address, v);
                        break;
                    }
            }
        }

        private static void ResetBlockBit(Computer cpu, uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)
        {
            var mem = cpu.MEMC;

            if (count == 0) count = 1;
            if (repeat == 0) repeat = 1;

            uint bitWidth = (uint)count * 8u;

            uint bitIndex;
            if (!sourceIsAddress)
            {
                // immediate 32-bit bit index
                bitIndex = valueOrAddress;
            }
            else
            {
                // REQUIRED SEMANTICS (mirrors ExSR pattern): bit index is a SINGLE BYTE at valueOrAddress
                bitIndex = mem.Get8bitFromRAM(valueOrAddress);
            }

            // With repeat we reset a run of bits starting at bitIndex
            for (uint i = 0; i < repeat; i++)
            {
                uint idx = (bitIndex + i) % bitWidth;
                ResetBitBigEndianInPlace(mem, address, count, (int)idx);
            }
        }

        private static void ResetBitBigEndianInPlace(MemoryController mem, uint baseAddr, byte countBytes, int bitIndex)
        {
            // Big-endian bit numbering: bitIndex 0 refers to MSB of first byte.
            int totalBits = countBytes * 8;
            if (totalBits <= 0) return;

            bitIndex %= totalBits;
            if (bitIndex < 0) bitIndex += totalBits;

            int byteIndex = bitIndex / 8;
            int bitInByte = bitIndex % 8;           // 0=MSB ... 7=LSB
            byte mask = (byte)(1 << (7 - bitInByte));

            uint addr = baseAddr + (uint)byteIndex;
            byte b = mem.Get8bitFromRAM(addr);
            mem.Set8bitToRAM(addr, (byte)(b & (byte)~mask));
        }

        // -------------------------
        // Core register bit-reset helper
        // -------------------------
        private static void ResetRegisterBit(Computer cpu, Width width, byte regIndex, byte rawBitIndex)
        {
            var regs = cpu.CPU.REGS;

            // Keep existing register helper semantics if present.
            // The Reset*BitBit methods are assumed to perform modulo and flag behavior as designed in REGs.
            switch (width)
            {
                case Width.Byte: regs.Reset8BitBit(regIndex, rawBitIndex); break;
                case Width.Word: regs.Reset16BitBit(regIndex, rawBitIndex); break;
                case Width.TriByte: regs.Reset24BitBit(regIndex, rawBitIndex); break;
                case Width.DWord: regs.Reset32BitBit(regIndex, rawBitIndex); break;
            }
        }

        private static uint OffsetAddress(uint baseAddr, int offset) => (uint)(baseAddr + offset);

        private static byte FloatToIndexByte(float f)
        {
            f = Math.Abs(f);
            if (float.IsNaN(f) || float.IsInfinity(f)) return 0;
            int n = (int)Math.Round(f);
            if (n < 0) return 0;
            if (n > 255) return 255;
            return (byte)n;
        }

        private static uint ResetBitU32(uint v, byte rawBitIndex)
        {
            int bitIndex = rawBitIndex & 31; // 0..31
            return v & ~(1u << bitIndex);
        }

        private static byte ReadRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x1F);
        private static byte ReadFloatRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x0F);

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
