using Continuum93.Emulator;
using Continuum93.Emulator.CPU;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExLD
    {
        static readonly Action<Computer>[] _dispatch = BuildDispatchTable();

        private static Action<Computer>[] BuildDispatchTable()
        {
            var table = new Action<Computer>[256];
            table[Instructions._r_n] = cpu => {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte dst = mem.Fetch(), v = mem.Fetch();
                regs.Set8BitRegister(dst, v);
            };
            table[Instructions._r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte r1Dest = mem.Fetch();
                byte r2Source = mem.Fetch();

                regs.Set8BitRegister(
                    r1Dest,
                    regs.Get8BitRegister(r2Source)
                );
            };
            table[Instructions._r_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte register = mem.Fetch();
                uint address = mem.Fetch24();

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM(address)
                );
            };
            table[Instructions._r_Innn_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                int offset = mem.Fetch24Signed();

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._r_Innn_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._r_Innn_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._r_Innn_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._r_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM(addressValue));
            };
            table[Instructions._r_Irrr_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._r_Irrr_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._r_Irrr_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._r_Irrr_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set8BitRegister(
                    register,
                    mem.Get8bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte register = mem.Fetch();
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                if (floatValue >= 256)
                {
                    flags.SetOverflow(true);
                    return;
                }

                flags.ClearOverflow();
                regs.Set8BitRegister(register, (byte)floatValue);
            };
            table[Instructions._rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                ushort regValue = mem.Fetch16();
                regs.Set16BitRegister(register, regValue);
            };
            table[Instructions._rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set16BitRegister(r1, regs.Get8BitRegister(r2));
            };
            table[Instructions._rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set16BitRegister(r1, regs.Get16BitRegister(r2));
            };
            table[Instructions._rr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                regs.Set16BitRegister(register, mem.Get16bitFromRAM(address));
            };
            table[Instructions._rr_Innn_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rr_Innn_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rr_Innn_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rr_Innn_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set16BitRegister(
                    r1,   // r1   - destination
                    mem.Get16bitFromRAM(regs.Get24BitRegister(r2)));     // r2 points to source
            };
            table[Instructions._rr_Irrr_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rr_Irrr_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rr_Irrr_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rr_Irrr_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set16BitRegister(
                    register,
                    mem.Get16bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regPointer = mem.Fetch();
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                if (floatValue >= 65536)
                {
                    flags.SetOverflow(true);
                    return;
                }

                flags.ClearOverflow();
                regs.Set16BitRegister(regPointer, (ushort)floatValue);
            };
            table[Instructions._rrr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint value = mem.Fetch24();
                regs.Set24BitRegister(register, value);
            };
            table[Instructions._rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set24BitRegister(r1, regs.Get8BitRegister(r2));
            };
            table[Instructions._rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set24BitRegister(r1, regs.Get16BitRegister(r2));
            };
            table[Instructions._rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set24BitRegister(r1, regs.Get24BitRegister(r2));
            };
            table[Instructions._rrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                regs.Set24BitRegister(register, mem.Get24bitFromRAM(address));
            };
            table[Instructions._rrr_Innn_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrr_Innn_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrr_Innn_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrr_Innn_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set24BitRegister(
                    r1,   // r1   - destination
                    mem.Get24bitFromRAM(regs.Get24BitRegister(r2)));     // r2 points to source
            };
            table[Instructions._rrr_Irrr_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrr_Irrr_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrr_Irrr_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrr_Irrr_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set24BitRegister(
                    register,
                    mem.Get24bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regPointer = mem.Fetch();
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                if (floatValue >= 0xFFFFFF)
                {
                    flags.SetOverflow(true);
                    return;
                }

                flags.ClearOverflow();
                regs.Set24BitRegister(regPointer, (uint)floatValue);
            };
            table[Instructions._rrrr_nnnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint value = mem.Fetch32();
                regs.Set32BitRegister(register, value);
            };
            table[Instructions._rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set32BitRegister(r1, regs.Get8BitRegister(r2));
            };
            table[Instructions._rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set32BitRegister(r1, regs.Get16BitRegister(r2));
            };
            table[Instructions._rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set32BitRegister(r1, regs.Get24BitRegister(r2));
            };
            table[Instructions._rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set32BitRegister(r1, regs.Get32BitRegister(r2));
            };
            table[Instructions._rrrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                regs.Set32BitRegister(register, mem.Get32bitFromRAM(address));
            };
            table[Instructions._rrrr_Innn_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrrr_Innn_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrrr_Innn_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrrr_Innn_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                uint address = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(address + offset))
                );
            };
            table[Instructions._rrrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                regs.Set32BitRegister(
                    r1,   // r1   - destination
                    mem.Get32bitFromRAM(regs.Get24BitRegister(r2)));     // r2 points to source
            };
            table[Instructions._rrrr_Irrr_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrrr_Irrr_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrrr_Irrr_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                short offset = regs.Get16BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrrr_Irrr_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = mem.Fetch();
                byte addressRegIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                uint addressValue = regs.Get24BitRegister(addressRegIndex);
                int offset = regs.Get24BitRegisterSigned(offsetIndex);

                regs.Set32BitRegister(
                    register,
                    mem.Get32bitFromRAM((uint)(addressValue + offset)));
            };
            table[Instructions._rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regPointer = mem.Fetch();
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                if (floatValue >= 0xFFFFFFFF)
                {
                    flags.SetOverflow(true);
                    return;
                }

                flags.ClearOverflow();
                regs.Set32BitRegister(regPointer, (uint)floatValue);
            };
            table[Instructions._InnnI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM(target, value, bytesToCopy);
            };
            table[Instructions._InnnI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte reg = mem.Fetch();
                mem.Set8bitToRAM(target,
                    regs.Get8BitRegister(reg)
                );
            };
            table[Instructions._InnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte reg = mem.Fetch();
                mem.Set16bitToRAM(target,
                    regs.Get16BitRegister(reg)
                );
            };
            table[Instructions._InnnI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte reg = mem.Fetch();
                mem.Set24bitToRAM(target,
                    regs.Get24BitRegister(reg)
                );
            };
            table[Instructions._InnnI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte reg = mem.Fetch();
                mem.Set32bitToRAM(target,
                    regs.Get32BitRegister(reg)
                );
            };
            table[Instructions._InnnI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint valueAddress = mem.Fetch24();
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);
            };
            table[Instructions._InnnI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                uint address = mem.Fetch24();
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam(address, floatValue);
            };
            table[Instructions._Innn_nnnI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);
            };
            table[Instructions._Innn_nnnI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                byte reg = mem.Fetch();
                mem.Set8bitToRAM((uint)(target + offset),
                    regs.Get8BitRegister(reg)
                );
            };
            table[Instructions._Innn_nnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                byte reg = mem.Fetch();
                mem.Set16bitToRAM((uint)(target + offset),
                    regs.Get16BitRegister(reg)
                );
            };
            table[Instructions._Innn_nnnI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                byte reg = mem.Fetch();
                mem.Set24bitToRAM((uint)(target + offset),
                    regs.Get24BitRegister(reg)
                );
            };
            table[Instructions._Innn_nnnI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                byte reg = mem.Fetch();
                mem.Set32bitToRAM((uint)(target + offset),
                    regs.Get32BitRegister(reg)
                );
            };
            table[Instructions._Innn_nnnI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_nnnI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                uint address = mem.Fetch24();
                int targetOffset = mem.Fetch24Signed();
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam((uint)(address + targetOffset), floatValue);
            };
            table[Instructions._Innn_rI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);
            };
            table[Instructions._Innn_rI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set8bitToRAM((uint)(target + offset),
                    regs.Get8BitRegister(reg)
                );
            };
            table[Instructions._Innn_rI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set16bitToRAM((uint)(target + offset),
                    regs.Get16BitRegister(reg)
                );
            };
            table[Instructions._Innn_rI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set24bitToRAM((uint)(target + offset),
                    regs.Get24BitRegister(reg)
                );
            };
            table[Instructions._Innn_rI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set32bitToRAM((uint)(target + offset),
                    regs.Get32BitRegister(reg)
                );
            };
            table[Instructions._Innn_rI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                var regs = cpu.CPU.REGS;

                uint address = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam((uint)(address + targetOffset), floatValue);
            };
            table[Instructions._Innn_rrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                var regs = cpu.CPU.REGS;

                uint address = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                sbyte targetOffset = regs.Get8BitRegisterSigned(targetOffsetIndex);
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam((uint)(address + targetOffset), floatValue);
            };
            table[Instructions._Innn_rrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);
            };
            table[Instructions._Innn_rrI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set8bitToRAM((uint)(target + offset),
                    regs.Get8BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set16bitToRAM((uint)(target + offset),
                    regs.Get16BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set24bitToRAM((uint)(target + offset),
                    regs.Get24BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set32bitToRAM((uint)(target + offset),
                    regs.Get32BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                uint address = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                short targetOffset = regs.Get16BitRegisterSigned(targetOffsetIndex);
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam((uint)(address + targetOffset), floatValue);
            };
            table[Instructions._Innn_rrrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);
            };
            table[Instructions._Innn_rrrI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set8bitToRAM((uint)(target + offset),
                    regs.Get8BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set16bitToRAM((uint)(target + offset),
                    regs.Get16BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set24bitToRAM((uint)(target + offset),
                    regs.Get24BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte reg = mem.Fetch();
                mem.Set32bitToRAM((uint)(target + offset),
                    regs.Get32BitRegister(reg)
                );
            };
            table[Instructions._Innn_rrrI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);
            };
            table[Instructions._Innn_rrrI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Innn_rrrI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                uint address = mem.Fetch24();
                byte targetOffsetIndex = mem.Fetch();
                int targetOffset = regs.Get24BitRegisterSigned(targetOffsetIndex);
                byte floatPointer = mem.Fetch();
                float floatValue = fregs.GetRegister(floatPointer);

                mem.SetFloatToRam((uint)(address + targetOffset), floatValue);


            };

            table[Instructions._IrrrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM(target, value, bytesToCopy);


            };
            table[Instructions._IrrrI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                mem.Set8bitToRAM(
                    regs.Get24BitRegister(r1),
                    regs.Get8BitRegister(r2)
                );

            };
            table[Instructions._IrrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                mem.Set16bitToRAM(
                    regs.Get24BitRegister(r1),
                    regs.Get16BitRegister(r2)
                );

            };
            table[Instructions._IrrrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                mem.Set24bitToRAM(
                    regs.Get24BitRegister(r1),
                    regs.Get24BitRegister(r2)
                );

            };
            table[Instructions._IrrrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte r2 = mem.Fetch();
                mem.Set32bitToRAM(
                    regs.Get24BitRegister(r1),
                    regs.Get32BitRegister(r2)
                );

            };
            table[Instructions._IrrrI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated(target, value, bytesToCopy, repeat);

            };
            table[Instructions._IrrrI_fr] = cpu => // LD (rrr), fr
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte regPointer = mem.Fetch();
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                uint address = regs.Get24BitRegister(regPointer);

                mem.SetFloatToRam(address, floatValue);


            };

            table[Instructions._Irrr_nnnI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                int targetOffset = mem.Fetch24Signed();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + targetOffset), value, bytesToCopy);


            };
            table[Instructions._Irrr_nnnI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                int targetOffset = mem.Fetch24Signed();
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + targetOffset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                int targetOffset = mem.Fetch24Signed();
                byte r2 = mem.Fetch();
                mem.Set8bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + targetOffset),
                    regs.Get8BitRegister(r2)
                );

            };
            table[Instructions._Irrr_nnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                int targetOffset = mem.Fetch24Signed();
                byte r2 = mem.Fetch();
                mem.Set16bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + targetOffset),
                    regs.Get16BitRegister(r2)
                );

            };
            table[Instructions._Irrr_nnnI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                int targetOffset = mem.Fetch24Signed();
                byte r2 = mem.Fetch();
                mem.Set24bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + targetOffset),
                    regs.Get24BitRegister(r2)
                );

            };
            table[Instructions._Irrr_nnnI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                int targetOffset = mem.Fetch24Signed();
                byte r2 = mem.Fetch();
                mem.Set32bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + targetOffset),
                    regs.Get32BitRegister(r2)
                );

            };
            table[Instructions._Irrr_nnnI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offsetValue = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                int offset = mem.Fetch24Signed();
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offsetValue));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_nnnI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte regPointer = mem.Fetch();
                int offset = mem.Fetch24Signed();
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                uint address = regs.Get24BitRegister(regPointer);

                mem.SetFloatToRam((uint)(address + offset), floatValue);


            };

            table[Instructions._Irrr_rI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);


            };
            table[Instructions._Irrr_rI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set8bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get8BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set16bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get16BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set24bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get24BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set32bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get32BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offset2Value = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get8BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get8BitRegisterSigned(offsetIndex);
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                uint address = regs.Get24BitRegister(regPointer);

                mem.SetFloatToRam((uint)(address + offset), floatValue);


            };

            table[Instructions._Irrr_rrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);


            };
            table[Instructions._Irrr_rrI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set8bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get8BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set16bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get16BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set24bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get24BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set32bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get32BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offset2Value = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get16BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get16BitRegisterSigned(offsetIndex);
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                uint address = regs.Get24BitRegister(regPointer);

                mem.SetFloatToRam((uint)(address + offset), floatValue);


            };

            table[Instructions._Irrr_rrrI_nnnn_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();

                mem.RAM.SetBytesToRAM((uint)(target + offset), value, bytesToCopy);


            };
            table[Instructions._Irrr_rrrI_nnnn_n_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                uint target = regs.Get24BitRegister(targetIndex);
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint value = mem.Fetch32();
                byte bytesToCopy = mem.Fetch();
                uint repeat = mem.Fetch24();

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set8bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get8BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set16bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get16BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set24bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get24BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte r1 = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte r2 = mem.Fetch();
                mem.Set32bitToRAM(
                    (uint)(regs.Get24BitRegister(r1) + offset),
                    regs.Get32BitRegister(r2)
                );

            };
            table[Instructions._Irrr_rrrI_InnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Innn_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                int offset2 = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Innn_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Innn_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Innn_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                uint valueAddress = mem.Fetch24();
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_IrrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                uint value = mem.Get32bitFromRAM(valueAddress);
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Irrr_nnnI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                int offset2Value = mem.Fetch24Signed();
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Irrr_rI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                sbyte offset2Value = regs.Get8BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Irrr_rrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                short offset2Value = regs.Get16BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_Irrr_rrrI_n_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte targetIndex = mem.Fetch();
                byte offset1Index = mem.Fetch();
                int offset1 = regs.Get24BitRegisterSigned(offset1Index);
                uint target = regs.Get24BitRegister(targetIndex);
                byte valueIndex = mem.Fetch();
                uint valueAddress = regs.Get24BitRegister(valueIndex);
                byte offset2Index = mem.Fetch();
                int offset2Value = regs.Get24BitRegisterSigned(offset2Index);
                uint value = mem.Get32bitFromRAM((uint)(valueAddress + offset2Value));
                byte bytesToCopy = mem.Fetch();
                byte repeatReg = mem.Fetch();
                uint repeat = regs.Get24BitRegister(repeatReg);

                mem.RAM.SetBytesToRAMRepeated((uint)(target + offset1), value, bytesToCopy, repeat);

            };
            table[Instructions._Irrr_rrrI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offset = regs.Get24BitRegisterSigned(offsetIndex);
                byte floatPointer = mem.Fetch();

                float floatValue = fregs.GetRegister(floatPointer);
                uint address = regs.Get24BitRegister(regPointer);

                mem.SetFloatToRam((uint)(address + offset), floatValue);


            };

            // Floating point LD
            table[Instructions._fr_nnnn] = cpu =>   // LD fr, nnn
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint floatBitValue = mem.Fetch32();
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_r] = cpu => // LD fr, r
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();

                fregs.SetRegister(
                    floatPointer,
                    regs.Get8BitRegister(regPointer)
                );


            };
            table[Instructions._fr_rr] = cpu => // LD fr, rr
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();

                fregs.SetRegister(
                    floatPointer,
                    regs.Get16BitRegister(regPointer)
                );


            };
            table[Instructions._fr_rrr] = cpu => // LD fr, rrr
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();

                fregs.SetRegister(
                    floatPointer,
                    regs.Get24BitRegister(regPointer)
                );


            };
            table[Instructions._fr_rrrr] = cpu => // LD fr, rrrr
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();

                fregs.SetRegister(
                    floatPointer,
                    regs.Get32BitRegister(regPointer)
                );


            };
            table[Instructions._fr_InnnI] = cpu => // LD fr, (nnn)
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint memAddress = mem.Fetch24();
                uint floatBitValue = mem.Get32bitFromRAM(memAddress);
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_Innn_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint memAddress = mem.Fetch24();
                int offset = mem.Fetch24Signed();
                uint floatBitValue = mem.Get32bitFromRAM((uint)(memAddress + offset));
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_Innn_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint memAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);
                uint floatBitValue = mem.Get32bitFromRAM((uint)(memAddress + offsetValue));
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_Innn_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint memAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);
                uint floatBitValue = mem.Get32bitFromRAM((uint)(memAddress + offsetValue));
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_Innn_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                uint memAddress = mem.Fetch24();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);
                uint floatBitValue = mem.Get32bitFromRAM((uint)(memAddress + offsetValue));
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                fregs.SetRegister(floatPointer, floatValue);


            };
            table[Instructions._fr_IrrrI] = cpu => // LD fr, (rrr)
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();

                byte[] floatBytes = mem.GetMemoryWrapped(regs.Get24BitRegister(regPointer), 4);

                fregs.SetRegister(
                    floatPointer,
                    FloatPointUtils.BytesToFloat(floatBytes)
                );


            };
            table[Instructions._fr_Irrr_nnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();
                int offset = mem.Fetch24Signed();

                byte[] floatBytes = mem.GetMemoryWrapped((uint)(regs.Get24BitRegister(regPointer) + offset), 4);

                fregs.SetRegister(
                    floatPointer,
                    FloatPointUtils.BytesToFloat(floatBytes)
                );


            };
            table[Instructions._fr_Irrr_rI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                sbyte offsetValue = regs.Get8BitRegisterSigned(offsetIndex);

                byte[] floatBytes = mem.GetMemoryWrapped((uint)(regs.Get24BitRegister(regPointer) + offsetValue), 4);

                fregs.SetRegister(
                    floatPointer,
                    FloatPointUtils.BytesToFloat(floatBytes)
                );


            };
            table[Instructions._fr_Irrr_rrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                short offsetValue = regs.Get16BitRegisterSigned(offsetIndex);

                byte[] floatBytes = mem.GetMemoryWrapped((uint)(regs.Get24BitRegister(regPointer) + offsetValue), 4);

                fregs.SetRegister(
                    floatPointer,
                    FloatPointUtils.BytesToFloat(floatBytes)
                );


            };
            table[Instructions._fr_Irrr_rrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte floatPointer = mem.Fetch();
                byte regPointer = mem.Fetch();
                byte offsetIndex = mem.Fetch();
                int offsetValue = regs.Get24BitRegisterSigned(offsetIndex);

                byte[] floatBytes = mem.GetMemoryWrapped((uint)(regs.Get24BitRegister(regPointer) + offsetValue), 4);

                fregs.SetRegister(
                    floatPointer,
                    FloatPointUtils.BytesToFloat(floatBytes)
                );


            };
            table[Instructions._fr_fr] = cpu =>  // LD fr, fr
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fReg1 = mem.Fetch();
                byte fReg2 = mem.Fetch();

                fregs.SetRegister(
                    fReg1,
                    fregs.GetRegister(fReg2)
                );


            };

            return table;
        }

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
