using Continuum93.Emulator;
using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    /// <summary>
    /// SL execution unit updated to use the shared Instructions sub-opcodes
    /// (the same addressing matrix used by LD / ADD / SUB / DIV / MUL ...).
    ///
    /// Implements all 244 sub-op variations (0..243) via a 256-entry dispatch table.
    /// Any unassigned entries are no-ops (safety).
    ///
    /// Semantics implemented here:
    /// - Register destinations: shift the destination register left by a shift-count
    ///   resolved from immediate / register / memory / float-reg (per sub-op).
    /// - Memory destinations: shift the value stored at the resolved address left by
    ///   a shift-count resolved from register / memory / float-reg (per sub-op).
    /// - Block destinations:
    ///     * Immediate count: shift the block (big-endian unsigned integer of length "count" bytes)
    ///       left by an immediate 32-bit shift count (modulo block bit width), with optional repeat.
    ///     * Memory-sourced count: shift the target block left by a shift count read from the
    ///       source block (big-endian unsigned of the same byte length), modulo block bit width,
    ///       repeated "repeat" times.
    /// - Float registers (_fr_*): treated as bitwise shift of the IEEE754 payload (uint bits),
    ///   returning a float from shifted bits. (If your ISA defines something else, adjust here.)
    /// </summary>
    public static class ExSL
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
                ShiftRegisterLeft(cpu, Width.Byte, dest, count);
            };

            table[Instructions._r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.Byte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._r_InnnI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressAbs);
            table[Instructions._r_Innn_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressAbsOffsetImm);
            table[Instructions._r_Innn_rI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg8);
            table[Instructions._r_Innn_rrI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg16);
            table[Instructions._r_Innn_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressAbsOffsetReg24);

            table[Instructions._r_IrrrI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressPtr);
            table[Instructions._r_Irrr_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressPtrOffsetImm);
            table[Instructions._r_Irrr_rI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg8);
            table[Instructions._r_Irrr_rrI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg16);
            table[Instructions._r_Irrr_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.Byte, AddressPtrOffsetReg24);

            table[Instructions._r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                ShiftRegisterLeft(cpu, Width.Byte, dest, count);
            };

            // 16-bit destination
            table[Instructions._rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                ushort imm = mem.Fetch16();
                ShiftRegisterLeft(cpu, Width.Word, dest, (byte)imm);
            };

            table[Instructions._rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.Word, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.Word, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rr_InnnI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressAbs);
            table[Instructions._rr_Innn_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressAbsOffsetImm);
            table[Instructions._rr_Innn_rI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg8);
            table[Instructions._rr_Innn_rrI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg16);
            table[Instructions._rr_Innn_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressAbsOffsetReg24);

            table[Instructions._rr_IrrrI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressPtr);
            table[Instructions._rr_Irrr_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressPtrOffsetImm);
            table[Instructions._rr_Irrr_rI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg8);
            table[Instructions._rr_Irrr_rrI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg16);
            table[Instructions._rr_Irrr_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.Word, AddressPtrOffsetReg24);

            table[Instructions._rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                ShiftRegisterLeft(cpu, Width.Word, dest, count);
            };

            // 24-bit destination
            table[Instructions._rrr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm24 = mem.Fetch24();
                ShiftRegisterLeft(cpu, Width.TriByte, dest, (byte)imm24);
            };

            table[Instructions._rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.TriByte, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.TriByte, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.TriByte, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrr_InnnI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressAbs);
            table[Instructions._rrr_Innn_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetImm);
            table[Instructions._rrr_Innn_rI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg8);
            table[Instructions._rrr_Innn_rrI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg16);
            table[Instructions._rrr_Innn_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressAbsOffsetReg24);

            table[Instructions._rrr_IrrrI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressPtr);
            table[Instructions._rrr_Irrr_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetImm);
            table[Instructions._rrr_Irrr_rI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg8);
            table[Instructions._rrr_Irrr_rrI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg16);
            table[Instructions._rrr_Irrr_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.TriByte, AddressPtrOffsetReg24);

            table[Instructions._rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                ShiftRegisterLeft(cpu, Width.TriByte, dest, count);
            };

            // 32-bit destination
            table[Instructions._rrrr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                byte dest = ReadRegIndex(mem);
                uint imm32 = mem.Fetch32();
                ShiftRegisterLeft(cpu, Width.DWord, dest, (byte)imm32);
            };

            table[Instructions._rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.DWord, dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get16BitRegister(src));
            };

            table[Instructions._rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get24BitRegister(src));
            };

            table[Instructions._rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);
                ShiftRegisterLeft(cpu, Width.DWord, dest, (byte)regs.Get32BitRegister(src));
            };

            table[Instructions._rrrr_InnnI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressAbs);
            table[Instructions._rrrr_Innn_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressAbsOffsetImm);
            table[Instructions._rrrr_Innn_rI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg8);
            table[Instructions._rrrr_Innn_rrI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg16);
            table[Instructions._rrrr_Innn_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressAbsOffsetReg24);

            table[Instructions._rrrr_IrrrI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressPtr);
            table[Instructions._rrrr_Irrr_nnnI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressPtrOffsetImm);
            table[Instructions._rrrr_Irrr_rI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg8);
            table[Instructions._rrrr_Irrr_rrI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg16);
            table[Instructions._rrrr_Irrr_rrrI] = cpu => ShiftRegFromMem8(cpu, Width.DWord, AddressPtrOffsetReg24);

            table[Instructions._rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);
                byte count = FloatToCountByte(fregs.GetRegister(fIdx));
                ShiftRegisterLeft(cpu, Width.DWord, dest, count);
            };
        }

        private static void ShiftRegFromMem8(Computer cpu, Width destWidth, AddressResolver resolver)
        {
            var mem = cpu.MEMC;
            var regs = cpu.CPU.REGS;
            byte dest = ReadRegIndex(mem);
            uint addr = resolver(mem, regs);
            byte count = mem.Get8bitFromRAM(addr);
            ShiftRegisterLeft(cpu, destWidth, dest, count);
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
                // Block immediate shift (count bytes, shiftCount=Fetch32, count=Fetch, repeat=1)
                table[group.BlockImmediateOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint shiftCount = mem.Fetch32();
                    byte count = mem.Fetch();
                    return (target, shiftCount, count, 1u, false);
                });

                // Block immediate shift with repeat (repeat=Fetch24)
                table[group.BlockImmediateRepeatOpcode] = BlockHandler((mem, regs) =>
                {
                    uint target = group.AddressResolver(mem, regs);
                    uint shiftCount = mem.Fetch32();
                    byte count = mem.Fetch();
                    uint repeat = mem.Fetch24();
                    return (target, shiftCount, count, repeat, false);
                });

                // Register->memory shifts (memory value shifted by register value)
                table[group.ByteRegOpcode] = MemHandler(Width.Byte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get8BitRegister(ReadRegIndex(mem))));
                table[group.WordRegOpcode] = MemHandler(Width.Word, (mem, regs) => (group.AddressResolver(mem, regs), (uint)regs.Get16BitRegister(ReadRegIndex(mem))));
                table[group.TriRegOpcode] = MemHandler(Width.TriByte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get24BitRegister(ReadRegIndex(mem))));
                table[group.DWordRegOpcode] = MemHandler(Width.DWord, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get32BitRegister(ReadRegIndex(mem))));

                // Memory-sourced block shifts with repeat in rrr (same shape as ExADD)
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

                // Float->memory shifts: treat memory float as bits, shift by float-reg-derived count
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
                    bits = ShiftLeftU32(bits, count);

                    mem.SetFloatToRam(address, FloatPointUtils.UintToFloat(bits));
                };
            }
        }

        // -------------------------
        // Float register destinations (_fr_*)
        // -------------------------
        private static void RegisterFloatRegisterHandlers(Action<Computer>[] table)
        {
            // SL fr, fr  (bitwise on IEEE754 bits)
            table[Instructions._fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                byte count = FloatToCountByte(fregs.GetRegister(src));
                bits = ShiftLeftU32(bits, count);

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // SL fr, nnnn  (count from 32-bit immediate)
            table[Instructions._fr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint shiftCount = mem.Fetch32();

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                bits = ShiftLeftU32(bits, (byte)(shiftCount & 0xFF));
                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // SL fr, r / rr / rrr / rrrr  (count from integer regs; low byte used)
            table[Instructions._fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                bits = ShiftLeftU32(bits, regs.Get8BitRegister(src));
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
                bits = ShiftLeftU32(bits, (byte)regs.Get16BitRegister(src));
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
                bits = ShiftLeftU32(bits, (byte)regs.Get24BitRegister(src));
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
                bits = ShiftLeftU32(bits, (byte)regs.Get32BitRegister(src));
                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };

            // SL fr, (mem8) via the shared address resolvers (count read as 8-bit from memory)
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
                bits = ShiftLeftU32(bits, count);
                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(bits));
            };
        }

        // -------------------------
        // Helpers: memory/block handlers (patterned after ExADD)
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

        private static Action<Computer> MemHandler(Width width, Func<MemoryController, Registers, (uint address, uint shiftCount)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, shiftCount) = resolver(mem, regs);
                ShiftMemoryValue(cpu, width, address, (byte)(shiftCount & 0xFF));
            };
        }

        private static Action<Computer> BlockHandler(Func<MemoryController, Registers, (uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, valueOrAddress, count, repeat, sourceIsAddress) = resolver(mem, regs);
                ShiftBlock(cpu, address, valueOrAddress, count, repeat, sourceIsAddress);
            };
        }

        private static void ShiftMemoryValue(Computer cpu, Width width, uint address, byte rawCount)
        {
            var mem = cpu.MEMC;
            int bits = width == Width.Byte ? 8 :
                       width == Width.Word ? 16 :
                       width == Width.TriByte ? 24 : 32;

            byte count = (byte)(rawCount % bits);

            switch (width)
            {
                case Width.Byte:
                    {
                        byte v = mem.Get8bitFromRAM(address);
                        mem.Set8bitToRAM(address, (byte)(v << count));
                        break;
                    }
                case Width.Word:
                    {
                        ushort v = mem.Get16bitFromRAM(address);
                        mem.Set16bitToRAM(address, (ushort)(v << count));
                        break;
                    }
                case Width.TriByte:
                    {
                        uint v = mem.Get24bitFromRAM(address) & 0xFFFFFFu;
                        uint r = (v << count) & 0xFFFFFFu;
                        mem.Set24bitToRAM(address, r);
                        break;
                    }
                case Width.DWord:
                    {
                        uint v = mem.Get32bitFromRAM(address);
                        mem.Set32bitToRAM(address, v << count);
                        break;
                    }
            }
        }

        private static void ShiftBlock(Computer cpu, uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)
        {
            var mem = cpu.MEMC;

            if (count == 0) count = 1;
            if (repeat == 0) repeat = 1;

            uint bitWidth = (uint)count * 8u;

            uint shiftCountBits;

            if (!sourceIsAddress)
            {
                // immediate 32-bit bit-count (as-is)
                shiftCountBits = valueOrAddress;
            }
            else
            {
                // REQUIRED SEMANTICS: shift count is a SINGLE BYTE at valueOrAddress
                shiftCountBits = mem.Get8bitFromRAM(valueOrAddress);
            }

            // REQUIRED SEMANTICS: apply it "repeat" times -> total bits shifted = shiftCountBits * repeat
            uint totalShiftBits = shiftCountBits * repeat;

            if (totalShiftBits == 0)
                return;

            if (totalShiftBits >= bitWidth)
            {
                // zero-fill entire block
                for (int i = 0; i < count; i++)
                    mem.Set8bitToRAM(address + (uint)i, 0);
                return;
            }

            ShiftLeftBigEndianInPlace(mem, address, count, (int)totalShiftBits);
        }



        private static uint ReadBigEndian(MemoryController mem, uint baseAddr, byte count)
        {
            // Returns up to 32-bit worth of big-endian bytes. If count>4, only the least significant 4 bytes are used.
            // (This keeps behavior deterministic without depending on BigInteger.)
            uint result = 0;
            int start = Math.Max(0, count - 4);
            for (int i = start; i < count; i++)
            {
                result = (result << 8) | mem.Get8bitFromRAM(baseAddr + (uint)i);
            }
            return result;
        }

        private static void ShiftLeftBigEndianInPlace(MemoryController mem, uint baseAddr, byte count, int shift)
        {
            // Shift a big-endian byte array left by "shift" bits in-place.
            int byteShift = shift / 8;
            int bitShift = shift % 8;

            // Read original
            byte[] buf = new byte[count];
            for (int i = 0; i < count; i++)
                buf[i] = mem.Get8bitFromRAM(baseAddr + (uint)i);

            // First apply whole-byte shift
            byte[] tmp = new byte[count];
            for (int i = 0; i < count; i++)
            {
                int src = i + byteShift;
                tmp[i] = (src < count) ? buf[src] : (byte)0;
            }

            // Then apply bit shift
            if (bitShift != 0)
            {
                byte carry = 0;
                for (int i = count - 1; i >= 0; i--)
                {
                    byte b = tmp[i];
                    byte newCarry = (byte)(b >> (8 - bitShift));
                    tmp[i] = (byte)((b << bitShift) | carry);
                    carry = newCarry;
                }
            }

            // Write back
            for (int i = 0; i < count; i++)
                mem.Set8bitToRAM(baseAddr + (uint)i, tmp[i]);
        }

        // -------------------------
        // Core register shift helper
        // -------------------------
        private static void ShiftRegisterLeft(Computer cpu, Width width, byte regIndex, byte rawCount)
        {
            var regs = cpu.CPU.REGS;
            switch (width)
            {
                case Width.Byte: regs.Shift8BitRegisterLeft(regIndex, rawCount); break;
                case Width.Word: regs.Shift16BitRegisterLeft(regIndex, rawCount); break;
                case Width.TriByte: regs.Shift24BitRegisterLeft(regIndex, rawCount); break;
                case Width.DWord: regs.Shift32BitRegisterLeft(regIndex, rawCount); break;
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

        private static uint ShiftLeftU32(uint v, byte rawCount)
        {
            byte count = (byte)(rawCount & 31); // 0..31
            return v << count;
        }

        private static byte ReadRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x1F);
        private static byte ReadFloatRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x0F);

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
