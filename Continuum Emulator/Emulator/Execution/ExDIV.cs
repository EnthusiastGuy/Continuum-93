using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;
using System.Linq;
using System.Numerics;

namespace Continuum93.Emulator.Execution
{
    /// <summary>
    /// DIV execution unit aligned with the shared Instructions sub-opcodes
    /// (the same addressing matrix used by LD/ADD/SUB).
    ///
    /// - Integer division is unsigned and stores the quotient back into the destination.
    /// - Division by zero (integer): destination becomes max for its width, carry flag is set.
    /// - Division by zero (float): runtime error.
    /// - The legacy "DIV ..., ..., remainder" variants are intentionally not implemented here.
    /// </summary>
    public static class ExDIV
    {
        private enum Width : byte
        {
            Byte = 1,
            Word = 2,
            TriByte = 3,
            DWord = 4
        }

        static readonly Action<Computer>[] _dispatch = BuildDispatchTable();

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

        private static void RegisterRegisterDestinations(Action<Computer>[] table)
        {
            // 8-bit
            table[Instructions._r_n] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Fetch()));
            table[Instructions._r_r] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), regs.Get8BitRegister(ReadRegIndex(mem))));
            table[Instructions._r_InnnI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(mem.Fetch24())));
            table[Instructions._r_Innn_nnnI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressAbsOffsetImm(mem, regs))));
            table[Instructions._r_Innn_rI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressAbsOffsetReg8(mem, regs))));
            table[Instructions._r_Innn_rrI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressAbsOffsetReg16(mem, regs))));
            table[Instructions._r_Innn_rrrI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressAbsOffsetReg24(mem, regs))));
            table[Instructions._r_IrrrI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressPtr(mem, regs))));
            table[Instructions._r_Irrr_nnnI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressPtrOffsetImm(mem, regs))));
            table[Instructions._r_Irrr_rI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressPtrOffsetReg8(mem, regs))));
            table[Instructions._r_Irrr_rrI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressPtrOffsetReg16(mem, regs))));
            table[Instructions._r_Irrr_rrrI] = RegHandler(Width.Byte, (mem, regs) => (ReadRegIndex(mem), mem.Get8bitFromRAM(AddressPtrOffsetReg24(mem, regs))));
            table[Instructions._r_fr] = cpu => IntegerDivByFloat(cpu, Width.Byte);

            // 16-bit
            table[Instructions._rr_nn] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Fetch16()));
            table[Instructions._rr_r] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), regs.Get8BitRegister(ReadRegIndex(mem))));
            table[Instructions._rr_rr] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), regs.Get16BitRegister(ReadRegIndex(mem))));
            table[Instructions._rr_InnnI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(mem.Fetch24())));
            table[Instructions._rr_Innn_nnnI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressAbsOffsetImm(mem, regs))));
            table[Instructions._rr_Innn_rI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressAbsOffsetReg8(mem, regs))));
            table[Instructions._rr_Innn_rrI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressAbsOffsetReg16(mem, regs))));
            table[Instructions._rr_Innn_rrrI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressAbsOffsetReg24(mem, regs))));
            table[Instructions._rr_IrrrI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressPtr(mem, regs))));
            table[Instructions._rr_Irrr_nnnI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressPtrOffsetImm(mem, regs))));
            table[Instructions._rr_Irrr_rI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressPtrOffsetReg8(mem, regs))));
            table[Instructions._rr_Irrr_rrI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressPtrOffsetReg16(mem, regs))));
            table[Instructions._rr_Irrr_rrrI] = RegHandler(Width.Word, (mem, regs) => (ReadRegIndex(mem), mem.Get16bitFromRAM(AddressPtrOffsetReg24(mem, regs))));
            table[Instructions._rr_fr] = cpu => IntegerDivByFloat(cpu, Width.Word);

            // 24-bit
            table[Instructions._rrr_nnn] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Fetch24()));
            table[Instructions._rrr_r] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), regs.Get8BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrr_rr] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), regs.Get16BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrr_rrr] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), regs.Get24BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrr_InnnI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(mem.Fetch24())));
            table[Instructions._rrr_Innn_nnnI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressAbsOffsetImm(mem, regs))));
            table[Instructions._rrr_Innn_rI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressAbsOffsetReg8(mem, regs))));
            table[Instructions._rrr_Innn_rrI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressAbsOffsetReg16(mem, regs))));
            table[Instructions._rrr_Innn_rrrI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressAbsOffsetReg24(mem, regs))));
            table[Instructions._rrr_IrrrI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressPtr(mem, regs))));
            table[Instructions._rrr_Irrr_nnnI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressPtrOffsetImm(mem, regs))));
            table[Instructions._rrr_Irrr_rI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressPtrOffsetReg8(mem, regs))));
            table[Instructions._rrr_Irrr_rrI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressPtrOffsetReg16(mem, regs))));
            table[Instructions._rrr_Irrr_rrrI] = RegHandler(Width.TriByte, (mem, regs) => (ReadRegIndex(mem), mem.Get24bitFromRAM(AddressPtrOffsetReg24(mem, regs))));
            table[Instructions._rrr_fr] = cpu => IntegerDivByFloat(cpu, Width.TriByte);

            // 32-bit
            table[Instructions._rrrr_nnnn] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Fetch32()));
            table[Instructions._rrrr_r] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), regs.Get8BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrrr_rr] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), regs.Get16BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrrr_rrr] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), regs.Get24BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrrr_rrrr] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), regs.Get32BitRegister(ReadRegIndex(mem))));
            table[Instructions._rrrr_InnnI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(mem.Fetch24())));
            table[Instructions._rrrr_Innn_nnnI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressAbsOffsetImm(mem, regs))));
            table[Instructions._rrrr_Innn_rI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressAbsOffsetReg8(mem, regs))));
            table[Instructions._rrrr_Innn_rrI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressAbsOffsetReg16(mem, regs))));
            table[Instructions._rrrr_Innn_rrrI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressAbsOffsetReg24(mem, regs))));
            table[Instructions._rrrr_IrrrI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressPtr(mem, regs))));
            table[Instructions._rrrr_Irrr_nnnI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressPtrOffsetImm(mem, regs))));
            table[Instructions._rrrr_Irrr_rI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressPtrOffsetReg8(mem, regs))));
            table[Instructions._rrrr_Irrr_rrI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressPtrOffsetReg16(mem, regs))));
            table[Instructions._rrrr_Irrr_rrrI] = RegHandler(Width.DWord, (mem, regs) => (ReadRegIndex(mem), mem.Get32bitFromRAM(AddressPtrOffsetReg24(mem, regs))));
            table[Instructions._rrrr_fr] = cpu => IntegerDivByFloat(cpu, Width.DWord);
        }

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
                // Immediate block divide
                table[group.BlockImmediateOpcode] = BlockHandler((mem, regs) =>
                {
                    uint address = group.AddressResolver(mem, regs);
                    uint value = mem.Fetch32();
                    byte count = mem.Fetch();
                    return (address, value, count, 1u, false);
                });

                table[group.BlockImmediateRepeatOpcode] = BlockHandler((mem, regs) =>
                {
                    uint address = group.AddressResolver(mem, regs);
                    uint value = mem.Fetch32();
                    byte count = mem.Fetch();
                    uint repeat = mem.Fetch24();
                    return (address, value, count, repeat, false);
                });

                // Register to memory divide
                table[group.ByteRegOpcode] = MemHandler(Width.Byte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get8BitRegister(ReadRegIndex(mem))));
                table[group.WordRegOpcode] = MemHandler(Width.Word, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get16BitRegister(ReadRegIndex(mem))));
                table[group.TriRegOpcode] = MemHandler(Width.TriByte, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get24BitRegister(ReadRegIndex(mem))));
                table[group.DWordRegOpcode] = MemHandler(Width.DWord, (mem, regs) => (group.AddressResolver(mem, regs), regs.Get32BitRegister(ReadRegIndex(mem))));

                // Memory sourced block divides with repeat
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

                // Float to memory: (float)memory /= fReg
                table[group.FloatOpcode] = cpu =>
                {
                    var mem = cpu.MEMC;
                    var regs = cpu.CPU.REGS;
                    var fregs = cpu.CPU.FREGS;

                    uint address = group.AddressResolver(mem, regs);
                    byte fIndex = ReadFloatRegIndex(mem);
                    float divisor = fregs.GetRegister(fIndex);
                    if (divisor.Equals(0))
                    {
                        ErrorHandler.ReportRuntimeError("Division by zero");
                        return;
                    }

                    float current = mem.GetFloatFromRAM(address);
                    float result = current / divisor;
                    cpu.CPU.FLAGS.SetSignNegative(result < 0);
                    mem.SetFloatToRam(address, result);
                };
            }
        }

        private static void RegisterFloatRegisterHandlers(Action<Computer>[] table)
        {
            // Float register destinations
            table[Instructions._fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadFloatRegIndex(mem);

                float divisor = fregs.GetRegister(src);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }

                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                uint bits = mem.Fetch32();
                float divisor = FloatPointUtils.UintToFloat(bits);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }

                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);
                float divisor = regs.Get8BitRegister(src);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }
                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);
                float divisor = regs.Get16BitRegister(src);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }
                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);
                float divisor = regs.Get24BitRegister(src);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }
                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                byte dest = ReadFloatRegIndex(mem);
                byte src = ReadRegIndex(mem);
                float divisor = regs.Get32BitRegister(src);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }
                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };

            table[Instructions._fr_InnnI] = FloatRegMemHandler(AddressAbs);
            table[Instructions._fr_Innn_nnnI] = FloatRegMemHandler(AddressAbsOffsetImm);
            table[Instructions._fr_Innn_rI] = FloatRegMemHandler(AddressAbsOffsetReg8);
            table[Instructions._fr_Innn_rrI] = FloatRegMemHandler(AddressAbsOffsetReg16);
            table[Instructions._fr_Innn_rrrI] = FloatRegMemHandler(AddressAbsOffsetReg24);
            table[Instructions._fr_IrrrI] = FloatRegMemHandler(AddressPtr);
            table[Instructions._fr_Irrr_nnnI] = FloatRegMemHandler(AddressPtrOffsetImm);
            table[Instructions._fr_Irrr_rI] = FloatRegMemHandler(AddressPtrOffsetReg8);
            table[Instructions._fr_Irrr_rrI] = FloatRegMemHandler(AddressPtrOffsetReg16);
            table[Instructions._fr_Irrr_rrrI] = FloatRegMemHandler(AddressPtrOffsetReg24);
        }

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

        private static Action<Computer> RegHandler(Width width, Func<MemoryController, Registers, (byte dest, uint value)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (dest, value) = resolver(mem, regs);
                DivideRegister(cpu, width, dest, value);
            };
        }

        private static Action<Computer> MemHandler(Width width, Func<MemoryController, Registers, (uint address, uint value)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, value) = resolver(mem, regs);
                DivideMemory(cpu, width, address, value);
            };
        }

        private static Action<Computer> BlockHandler(Func<MemoryController, Registers, (uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)> resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var (address, valueOrAddress, count, repeat, sourceIsAddress) = resolver(mem, regs);
                DivideBlock(cpu, address, valueOrAddress, count, repeat, sourceIsAddress);
            };
        }

        private static Action<Computer> FloatRegMemHandler(AddressResolver resolver)
        {
            return cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte dest = ReadFloatRegIndex(mem);
                uint address = resolver(mem, regs);

                float divisor = mem.GetFloatFromRAM(address);
                if (divisor.Equals(0))
                {
                    ErrorHandler.ReportRuntimeError("Division by zero");
                    return;
                }

                float result = fregs.GetRegister(dest) / divisor;
                cpu.CPU.FLAGS.SetSignNegative(result < 0);
                fregs.SetRegister(dest, result);
            };
        }

        private static void DivideRegister(Computer cpu, Width width, byte dest, uint divisor)
        {
            var regs = cpu.CPU.REGS;
            var flags = cpu.CPU.FLAGS;

            if (divisor == 0)
            {
                switch (width)
                {
                    case Width.Byte: regs.Set8BitRegister(dest, 0xFF); break;
                    case Width.Word: regs.Set16BitRegister(dest, 0xFFFF); break;
                    case Width.TriByte: regs.Set24BitRegister(dest, 0xFFFFFF); break;
                    case Width.DWord: regs.Set32BitRegister(dest, 0xFFFFFFFF); break;
                }
                flags.SetCarry(true);
                return;
            }

            flags.SetCarry(false);
            switch (width)
            {
                case Width.Byte:
                    regs.Set8BitRegister(dest, (byte)(regs.Get8BitRegister(dest) / divisor));
                    break;
                case Width.Word:
                    regs.Set16BitRegister(dest, (ushort)(regs.Get16BitRegister(dest) / divisor));
                    break;
                case Width.TriByte:
                    regs.Set24BitRegister(dest, regs.Get24BitRegister(dest) / divisor);
                    break;
                case Width.DWord:
                    regs.Set32BitRegister(dest, regs.Get32BitRegister(dest) / divisor);
                    break;
            }
        }

        private static void DivideMemory(Computer cpu, Width width, uint address, uint divisor)
        {
            var mem = cpu.MEMC;
            var flags = cpu.CPU.FLAGS;

            if (divisor == 0)
            {
                switch (width)
                {
                    case Width.Byte: mem.Set8bitToRAM(address, 0xFF); break;
                    case Width.Word: mem.Set16bitToRAM(address, 0xFFFF); break;
                    case Width.TriByte: mem.Set24bitToRAM(address, 0xFFFFFF); break;
                    case Width.DWord: mem.Set32bitToRAM(address, 0xFFFFFFFF); break;
                }
                flags.SetCarry(true);
                return;
            }

            flags.SetCarry(false);
            switch (width)
            {
                case Width.Byte:
                    mem.Set8bitToRAM(address, (byte)(mem.Get8bitFromRAM(address) / divisor));
                    break;
                case Width.Word:
                    mem.Set16bitToRAM(address, (ushort)(mem.Get16bitFromRAM(address) / divisor));
                    break;
                case Width.TriByte:
                    mem.Set24bitToRAM(address, mem.Get24bitFromRAM(address) / divisor);
                    break;
                case Width.DWord:
                    mem.Set32bitToRAM(address, mem.Get32bitFromRAM(address) / divisor);
                    break;
            }
        }

        private static void DivideBlock(Computer cpu, uint address, uint valueOrAddress, byte count, uint repeat, bool sourceIsAddress)
        {
            var mem = cpu.MEMC;
            var flags = cpu.CPU.FLAGS;

            if (count == 0)
                count = 1;
            if (repeat == 0)
                repeat = 1;

            for (uint r = 0; r < repeat; r++)
            {
                byte[] dividendBytes = ReadBytes(mem, address, count);
                BigInteger dividend = FromBigEndianUnsigned(dividendBytes);

                BigInteger divisor = sourceIsAddress
                    ? FromBigEndianUnsigned(ReadBytes(mem, valueOrAddress, count))
                    : new BigInteger((ulong)valueOrAddress);

                if (divisor.IsZero)
                {
                    // set destination to max for its byte-length
                    for (int i = 0; i < count; i++)
                        mem.Set8bitToRAM(address + (uint)i, 0xFF);
                    flags.SetCarry(true);
                    continue;
                }

                BigInteger quotient = dividend / divisor;
                byte[] outBytes = ToBigEndianUnsignedFixed(quotient, count);
                for (int i = 0; i < count; i++)
                    mem.Set8bitToRAM(address + (uint)i, outBytes[i]);

                flags.SetCarry(false);
            }
        }

        private static void IntegerDivByFloat(Computer cpu, Width width)
        {
            var mem = cpu.MEMC;
            var regs = cpu.CPU.REGS;
            var fregs = cpu.CPU.FREGS;

            byte dest = ReadRegIndex(mem);
            byte fIdx = ReadFloatRegIndex(mem);
            float divisor = fregs.GetRegister(fIdx);

            if (divisor.Equals(0))
            {
                ErrorHandler.ReportRuntimeError("Division by zero");
                return;
            }

            switch (width)
            {
                case Width.Byte:
                {
                    float v = regs.Get8BitRegister(dest);
                    v /= divisor;
                    regs.Set8BitRegister(dest, (byte)Math.Round(v));
                    break;
                }
                case Width.Word:
                {
                    float v = regs.Get16BitRegister(dest);
                    v /= divisor;
                    regs.Set16BitRegister(dest, (ushort)Math.Round(v));
                    break;
                }
                case Width.TriByte:
                {
                    float v = regs.Get24BitRegister(dest);
                    v /= divisor;
                    uint q = (uint)Math.Abs(Math.Round(v));
                    regs.Set24BitRegister(dest, q);
                    break;
                }
                case Width.DWord:
                {
                    float v = regs.Get32BitRegister(dest);
                    v /= divisor;
                    uint q = (uint)Math.Abs(Math.Round(v));
                    regs.Set32BitRegister(dest, q);
                    break;
                }
            }
        }

        private static uint OffsetAddress(uint baseAddr, int offset) => (uint)(baseAddr + offset);

        private static byte[] ReadBytes(MemoryController mem, uint address, int count)
        {
            var bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = mem.Get8bitFromRAM(address + (uint)i);
            return bytes;
        }

        private static BigInteger FromBigEndianUnsigned(byte[] bigEndian)
        {
            // BigInteger expects little-endian two's complement.
            // Reverse and add a 0-byte to force unsigned interpretation.
            return new BigInteger(bigEndian.Reverse().Concat(new byte[] { 0 }).ToArray());
        }

        private static byte[] ToBigEndianUnsignedFixed(BigInteger value, int count)
        {
            if (value.Sign < 0)
                value = BigInteger.Zero;

            byte[] little = value.ToByteArray(); // little-endian, two's complement
            if (little.Length > 1 && little[^1] == 0)
            {
                // remove sign extension for positive numbers
                Array.Resize(ref little, little.Length - 1);
            }

            byte[] big = little.Reverse().ToArray();
            if (big.Length == count)
                return big;

            if (big.Length > count)
            {
                // take least-significant bytes (rightmost in big-endian)
                return big[^count..];
            }

            // left-pad with zeros
            byte[] padded = new byte[count];
            Buffer.BlockCopy(big, 0, padded, count - big.Length, big.Length);
            return padded;
        }

        private static byte ReadRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x1F);
        private static byte ReadFloatRegIndex(MemoryController mem) => (byte)(mem.Fetch() & 0x0F);

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
