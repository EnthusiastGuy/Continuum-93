using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    /// <summary>
    /// AND execution unit updated to use the shared Instructions sub-opcodes
    /// (the same addressing matrix used by LD / ADD / SUB / DIV / MUL / SL / SR / SET ...).
    ///
    /// Implements all 244 sub-op variations (0..243) via a 256-entry dispatch table.
    /// Any unassigned entries are no-ops (safety).
    ///
    /// Semantics implemented here (bitwise AND):
    /// - Register destinations: dest = dest & src, where src can be immediate / register / memory / float-reg (per sub-op).
    /// - Memory destinations:
    ///     * Scalar widths: (addr) = (addr) & src (src from integer reg or float reg, per opcode).
    ///     * Block forms (the same opcodes used as "block groups" in ExSET/ExSL/ExSR):
    ///         - Immediate mask: AND a byte-block of length "count" with a repeating 32-bit immediate pattern.
    ///         - Memory mask: AND a byte-block of length "count" with another byte-block at valueAddress.
    ///           With "repeat" (rrr) we process repeat blocks, advancing both src/dst by count bytes each time.
    /// - Float register destinations (_fr_*): treated as bitwise operation on the IEEE754 payload (uint bits),
    ///   AND-ing the dest float bits with the resolved source bits/mask.
    /// </summary>
    public static class ExAND
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
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte imm = mem.Fetch();

                regs.And8Bit(dest, imm);
            };

            table[Instructions._r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And8Bit(dest, regs.Get8BitRegister(src));
            };

            table[Instructions._r_InnnI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressAbs);
            table[Instructions._r_Innn_nnnI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressAbsOffsetImm);
            table[Instructions._r_Innn_rI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressAbsOffsetReg8);
            table[Instructions._r_Innn_rrI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressAbsOffsetReg16);
            table[Instructions._r_Innn_rrrI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressAbsOffsetReg24);

            table[Instructions._r_IrrrI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressPtr);
            table[Instructions._r_Irrr_nnnI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressPtrOffsetImm);
            table[Instructions._r_Irrr_rI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressPtrOffsetReg8);
            table[Instructions._r_Irrr_rrI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressPtrOffsetReg16);
            table[Instructions._r_Irrr_rrrI] = cpu => AndRegFromMem(cpu, Width.Byte, AddressPtrOffsetReg24);

            table[Instructions._r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(fIdx));
                regs.And8Bit(dest, (byte)(bits & 0xFF));
            };

            // 16-bit destination
            table[Instructions._rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                ushort imm = mem.Fetch16();

                regs.And16Bit(dest, imm);
            };

            table[Instructions._rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And16Bit(dest, (ushort)regs.Get8BitRegister(src));
            };

            table[Instructions._rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And16Bit(dest, regs.Get16BitRegister(src));
            };

            table[Instructions._rr_InnnI] = cpu => AndRegFromMem(cpu, Width.Word, AddressAbs);
            table[Instructions._rr_Innn_nnnI] = cpu => AndRegFromMem(cpu, Width.Word, AddressAbsOffsetImm);
            table[Instructions._rr_Innn_rI] = cpu => AndRegFromMem(cpu, Width.Word, AddressAbsOffsetReg8);
            table[Instructions._rr_Innn_rrI] = cpu => AndRegFromMem(cpu, Width.Word, AddressAbsOffsetReg16);
            table[Instructions._rr_Innn_rrrI] = cpu => AndRegFromMem(cpu, Width.Word, AddressAbsOffsetReg24);

            table[Instructions._rr_IrrrI] = cpu => AndRegFromMem(cpu, Width.Word, AddressPtr);
            table[Instructions._rr_Irrr_nnnI] = cpu => AndRegFromMem(cpu, Width.Word, AddressPtrOffsetImm);
            table[Instructions._rr_Irrr_rI] = cpu => AndRegFromMem(cpu, Width.Word, AddressPtrOffsetReg8);
            table[Instructions._rr_Irrr_rrI] = cpu => AndRegFromMem(cpu, Width.Word, AddressPtrOffsetReg16);
            table[Instructions._rr_Irrr_rrrI] = cpu => AndRegFromMem(cpu, Width.Word, AddressPtrOffsetReg24);

            table[Instructions._rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(fIdx));
                regs.And16Bit(dest, (ushort)(bits & 0xFFFF));
            };

            // 24-bit destination
            table[Instructions._rrr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                uint imm24 = mem.Fetch24() & 0xFFFFFFu;

                regs.And24Bit(dest, imm24);
            };

            table[Instructions._rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And24Bit(dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And24Bit(dest, regs.Get16BitRegister(src));
            };

            table[Instructions._rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And24Bit(dest, regs.Get24BitRegister(src) & 0xFFFFFFu);
            };

            table[Instructions._rrr_InnnI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressAbs);
            table[Instructions._rrr_Innn_nnnI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressAbsOffsetImm);
            table[Instructions._rrr_Innn_rI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressAbsOffsetReg8);
            table[Instructions._rrr_Innn_rrI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressAbsOffsetReg16);
            table[Instructions._rrr_Innn_rrrI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressAbsOffsetReg24);

            table[Instructions._rrr_IrrrI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressPtr);
            table[Instructions._rrr_Irrr_nnnI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressPtrOffsetImm);
            table[Instructions._rrr_Irrr_rI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressPtrOffsetReg8);
            table[Instructions._rrr_Irrr_rrI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressPtrOffsetReg16);
            table[Instructions._rrr_Irrr_rrrI] = cpu => AndRegFromMem(cpu, Width.TriByte, AddressPtrOffsetReg24);

            table[Instructions._rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(fIdx)) & 0xFFFFFFu;
                regs.And24Bit(dest, bits);
            };

            // 32-bit destination
            table[Instructions._rrrr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                uint imm32 = mem.Fetch32();

                regs.And32Bit(dest, imm32);
            };

            table[Instructions._rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And32Bit(dest, regs.Get8BitRegister(src));
            };

            table[Instructions._rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And32Bit(dest, regs.Get16BitRegister(src));
            };

            table[Instructions._rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And32Bit(dest, regs.Get24BitRegister(src) & 0xFFFFFFu);
            };

            table[Instructions._rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte dest = ReadRegIndex(mem);
                byte src = ReadRegIndex(mem);

                regs.And32Bit(dest, regs.Get32BitRegister(src));
            };

            table[Instructions._rrrr_InnnI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressAbs);
            table[Instructions._rrrr_Innn_nnnI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressAbsOffsetImm);
            table[Instructions._rrrr_Innn_rI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressAbsOffsetReg8);
            table[Instructions._rrrr_Innn_rrI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressAbsOffsetReg16);
            table[Instructions._rrrr_Innn_rrrI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressAbsOffsetReg24);

            table[Instructions._rrrr_IrrrI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressPtr);
            table[Instructions._rrrr_Irrr_nnnI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressPtrOffsetImm);
            table[Instructions._rrrr_Irrr_rI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressPtrOffsetReg8);
            table[Instructions._rrrr_Irrr_rrI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressPtrOffsetReg16);
            table[Instructions._rrrr_Irrr_rrrI] = cpu => AndRegFromMem(cpu, Width.DWord, AddressPtrOffsetReg24);

            table[Instructions._rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadRegIndex(mem);
                byte fIdx = ReadFloatRegIndex(mem);

                uint bits = FloatPointUtils.FloatToUint(fregs.GetRegister(fIdx));
                regs.And32Bit(dest, bits);
            };
        }

        private static void AndRegFromMem(Computer cpu, Width destWidth, AddressResolver resolver)
        {
            var mem = cpu.MEMC;
            var regs = cpu.CPU.REGS;

            byte dest = ReadRegIndex(mem);
            uint addr = resolver(mem, regs);

            switch (destWidth)
            {
                case Width.Byte:
                    regs.And8Bit(dest, mem.Get8bitFromRAM(addr));
                    break;
                case Width.Word:
                    regs.And16Bit(dest, mem.Get16bitFromRAM(addr));
                    break;
                case Width.TriByte:
                    regs.And24Bit(dest, mem.Get24bitFromRAM(addr) & 0xFFFFFFu);
                    break;
                case Width.DWord:
                    regs.And32Bit(dest, mem.Get32bitFromRAM(addr));
                    break;
            }
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
                // Block: immediate mask (uint32), count bytes, repeat=1
                table[group.BlockImmediateOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint target = group.AddressResolver(mem, regs);
                    uint imm32 = mem.Fetch32();
                    byte count = mem.Fetch();

                    AndBlockImmediate(cpu, target, imm32, count, 1u);
                };

                // Block: immediate mask (uint32), count bytes, repeat from imm24
                table[group.BlockImmediateRepeatOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint target = group.AddressResolver(mem, regs);
                    uint imm32 = mem.Fetch32();
                    byte count = mem.Fetch();
                    uint repeat = mem.Fetch24();

                    AndBlockImmediate(cpu, target, imm32, count, repeat);
                };

                // Register -> memory scalar AND
                table[group.ByteRegOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte src = ReadRegIndex(mem);

                    regs.And8BitRegToMem(address, regs.Get8BitRegister(src));
                };

                table[group.WordRegOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte src = ReadRegIndex(mem);

                    regs.And16BitRegToMem(address, regs.Get16BitRegister(src));
                };

                table[group.TriRegOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte src = ReadRegIndex(mem);

                    regs.And24BitRegToMem(address, regs.Get24BitRegister(src) & 0xFFFFFFu);
                };

                table[group.DWordRegOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte src = ReadRegIndex(mem);

                    regs.And32BitRegToMem(address, regs.Get32BitRegister(src));
                };

                // Memory-to-memory block AND (value address resolved by matrix; repeat from rrr)
                for (int i = 0; i < valueResolvers.Length; i++)
                {
                    byte opcode = group.ValueOpcodeMatrix[i];
                    AddressResolver valueResolver = valueResolvers[i];

                    table[opcode] = cpu =>
                    {
                        var mem = cpu.MEMC;
                        var regs = cpu.CPU.REGS;

                        uint target = group.AddressResolver(mem, regs);
                        uint valueAddress = valueResolver(mem, regs);
                        byte count = mem.Fetch();
                        uint repeat = regs.Get24BitRegister(ReadRegIndex(mem));

                        AndBlockMemToMem(cpu, target, valueAddress, count, repeat);
                    };
                }

                // Float -> memory AND (treat memory float as bits, AND with float-reg bits)
                table[group.FloatOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;
                    var fregs = cpu.CPU.FREGS;
                    var flags = cpu.CPU.FLAGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte fIndex = ReadFloatRegIndex(mem);

                    float current = mem.GetFloatFromRAM(address);
                    uint bits = FloatPointUtils.FloatToUint(current);

                    uint mask = FloatPointUtils.FloatToUint(fregs.GetRegister(fIndex));
                    bits &= mask;

                    mem.SetFloatToRam(address, FloatPointUtils.UintToFloat(bits));

                    flags.ClearCarry();
                    flags.SetZero(bits == 0);
                };
            }
        }

        // -------------------------
        // Float register destinations (_fr_*): bitwise on IEEE754 bits
        // -------------------------
        private static void RegisterFloatRegisterHandlers(Action<Computer>[] table)
        {
            // AND fr, fr
            table[Instructions._fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadFloatRegIndex(mem);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint sb = FloatPointUtils.FloatToUint(fregs.GetRegister(src));
                uint result = db & sb;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            // AND fr, nnnn
            table[Instructions._fr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                uint imm32 = mem.Fetch32();

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint result = db & imm32;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            // AND fr, r / rr / rrr / rrrr  (zero-extended mask)
            table[Instructions._fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint mask = regs.Get8BitRegister(src);
                uint result = db & mask;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            table[Instructions._fr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint mask = regs.Get16BitRegister(src);
                uint result = db & mask;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            table[Instructions._fr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint mask = regs.Get24BitRegister(src) & 0xFFFFFFu;
                uint result = db & mask;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            table[Instructions._fr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint mask = regs.Get32BitRegister(src);
                uint result = db & mask;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };

            // AND fr, (memFloat) via shared address resolvers
            table[Instructions._fr_InnnI] = FloatRegMemFloatHandler(AddressAbs);
            table[Instructions._fr_Innn_nnnI] = FloatRegMemFloatHandler(AddressAbsOffsetImm);
            table[Instructions._fr_Innn_rI] = FloatRegMemFloatHandler(AddressAbsOffsetReg8);
            table[Instructions._fr_Innn_rrI] = FloatRegMemFloatHandler(AddressAbsOffsetReg16);
            table[Instructions._fr_Innn_rrrI] = FloatRegMemFloatHandler(AddressAbsOffsetReg24);

            table[Instructions._fr_IrrrI] = FloatRegMemFloatHandler(AddressPtr);
            table[Instructions._fr_Irrr_nnnI] = FloatRegMemFloatHandler(AddressPtrOffsetImm);
            table[Instructions._fr_Irrr_rI] = FloatRegMemFloatHandler(AddressPtrOffsetReg8);
            table[Instructions._fr_Irrr_rrI] = FloatRegMemFloatHandler(AddressPtrOffsetReg16);
            table[Instructions._fr_Irrr_rrrI] = FloatRegMemFloatHandler(AddressPtrOffsetReg24);
        }

        private static Action<Computer> FloatRegMemFloatHandler(AddressResolver resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte dest = ReadFloatRegIndex(mem);
                uint addr = resolver(mem, regs);

                uint db = FloatPointUtils.FloatToUint(fregs.GetRegister(dest));
                uint sb = FloatPointUtils.FloatToUint(mem.GetFloatFromRAM(addr));
                uint result = db & sb;

                fregs.SetRegister(dest, FloatPointUtils.UintToFloat(result));

                flags.ClearCarry();
                flags.SetZero(result == 0);
            };
        }

        // -------------------------
        // Block helpers
        // -------------------------
        private static void AndBlockImmediate(Computer cpu, uint address, uint imm32, byte count, uint repeat)
        {
            var mem = cpu.MEMC;
            var flags = cpu.CPU.FLAGS;

            if (count == 0) count = 1;
            if (repeat == 0) repeat = 1;

            bool allZero = true;

            for (uint block = 0; block < repeat; block++)
            {
                uint baseAddr = address + (uint)(block * count);

                for (int i = 0; i < count; i++)
                {
                    uint a = baseAddr + (uint)i;

                    byte b = mem.Get8bitFromRAM(a);
                    byte m = (byte)((imm32 >> (8 * (i & 3))) & 0xFF); // repeat 4-byte pattern (little-endian)
                    byte result = (byte)(b & m);
                    mem.Set8bitToRAM(a, result);

                    if (result != 0) allZero = false;
                }
            }

            flags.ClearCarry();
            flags.SetZero(allZero);
        }

        private static void AndBlockMemToMem(Computer cpu, uint destAddress, uint srcAddress, byte count, uint repeat)
        {
            var mem = cpu.MEMC;
            var flags = cpu.CPU.FLAGS;

            if (count == 0) count = 1;
            if (repeat == 0) repeat = 1;

            bool allZero = true;

            for (uint block = 0; block < repeat; block++)
            {
                uint dBase = destAddress + (uint)(block * count);
                uint sBase = srcAddress + (uint)(block * count);

                for (int i = 0; i < count; i++)
                {
                    uint da = dBase + (uint)i;
                    uint sa = sBase + (uint)i;

                    byte d = mem.Get8bitFromRAM(da);
                    byte s = mem.Get8bitFromRAM(sa);
                    byte result = (byte)(d & s);

                    mem.Set8bitToRAM(da, result);

                    if (result != 0) allZero = false;
                }
            }

            flags.ClearCarry();
            flags.SetZero(allZero);
        }

        // -------------------------
        // Addressing helpers (shared matrix)
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

        private static uint OffsetAddress(uint baseAddr, int offset) => (uint)(baseAddr + offset);

        private static byte ReadRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x1F);
        private static byte ReadFloatRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x0F);

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
