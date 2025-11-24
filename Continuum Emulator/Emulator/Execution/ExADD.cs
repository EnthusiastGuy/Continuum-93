using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExADD
    {
        static readonly Action<Computer>[] _dispatch = BuildDispatchTable();

        private static Action<Computer>[] BuildDispatchTable()
        {
            var table = new Action<Computer>[256];

            // -----------------------
            // Integer register ADDs
            // -----------------------

            table[Mnem.ADDSUB_r_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                byte regValue = mem.Fetch();

                regs.AddTo8BitRegister(register, regValue);
            };

            table[Mnem.ADDSUB_r_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo8BitRegister(
                    reg1,
                    regs.Get8BitRegister(reg2));
            };

            table[Mnem.ADDSUB_r_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo8BitRegister(
                    register,
                    mem.Get8bitFromRAM(address));
            };

            table[Mnem.ADDSUB_r_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo8BitRegister(
                    reg1,
                    mem.Get8bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB_rr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                ushort regValue = mem.Fetch();

                regs.AddTo16BitRegister(register, regValue);
            };

            table[Mnem.ADDSUB16_rr_nn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                ushort regValue = mem.Fetch16();

                regs.AddTo16BitRegister(register, regValue);
            };

            table[Mnem.ADDSUB_rr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo16BitRegister(reg1, regs.Get8BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo16BitRegister(reg1, regs.Get16BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo16BitRegister(
                    register,
                    mem.Get8bitFromRAM(address));
            };

            table[Mnem.ADDSUB16_rr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo16BitRegister(
                    register,
                    mem.Get16bitFromRAM(address));
            };

            table[Mnem.ADDSUB_rr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo16BitRegister(
                    reg1,
                    mem.Get8bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB16_rr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo16BitRegister(
                    reg1,
                    mem.Get16bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB_rrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo24BitRegister(reg, mem.Fetch());
            };

            table[Mnem.ADDSUB16_rrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo24BitRegister(reg, mem.Fetch16());
            };

            table[Mnem.ADDSUB24_rrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo24BitRegister(reg, mem.Fetch24());
            };

            table[Mnem.ADDSUB_rrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(reg1, regs.Get8BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(reg1, regs.Get16BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(reg1, regs.Get24BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo24BitRegister(
                    register,
                    mem.Get8bitFromRAM(address));
            };

            table[Mnem.ADDSUB16_rrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo24BitRegister(
                    register,
                    mem.Get16bitFromRAM(address));
            };

            table[Mnem.ADDSUB24_rrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo24BitRegister(
                    register,
                    mem.Get24bitFromRAM(address));
            };

            table[Mnem.ADDSUB_rrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(
                    reg1,
                    mem.Get8bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB16_rrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(
                    reg1,
                    mem.Get16bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB24_rrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo24BitRegister(
                    reg1,
                    mem.Get24bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB_rrrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo32BitRegister(reg, mem.Fetch());
            };

            table[Mnem.ADDSUB16_rrrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo32BitRegister(reg, mem.Fetch16());
            };

            table[Mnem.ADDSUB24_rrrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo32BitRegister(reg, mem.Fetch24());
            };

            table[Mnem.ADDSUB32_rrrr_n] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg = (byte)(mem.Fetch() & 0b00011111);
                regs.AddTo32BitRegister(reg, mem.Fetch32());
            };

            table[Mnem.ADDSUB_rrrr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(reg1, regs.Get8BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrrr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(reg1, regs.Get16BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrrr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(reg1, regs.Get24BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrrr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(reg1, regs.Get32BitRegister(reg2));
            };

            table[Mnem.ADDSUB_rrrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo32BitRegister(
                    register,
                    mem.Get8bitFromRAM(address));
            };

            table[Mnem.ADDSUB16_rrrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo32BitRegister(
                    register,
                    mem.Get16bitFromRAM(address));
            };

            table[Mnem.ADDSUB24_rrrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo32BitRegister(
                    register,
                    mem.Get24bitFromRAM(address));
            };

            table[Mnem.ADDSUB32_rrrr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte register = (byte)(mem.Fetch() & 0b00011111);
                uint address = mem.Fetch24();

                regs.AddTo32BitRegister(
                    register,
                    mem.Get32bitFromRAM(address));
            };

            table[Mnem.ADDSUB_rrrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(
                    reg1,
                    mem.Get8bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB16_rrrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(
                    reg1,
                    mem.Get16bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB24_rrrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(
                    reg1,
                    mem.Get24bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            table[Mnem.ADDSUB32_rrrr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                byte reg1 = (byte)(mem.Fetch() & 0b00011111);
                byte reg2 = (byte)(mem.Fetch() & 0b00011111);

                regs.AddTo32BitRegister(
                    reg1,
                    mem.Get32bitFromRAM(regs.Get24BitRegister(reg2)));
            };

            // -----------------------
            // ADD to memory (absolute)
            // -----------------------

            table[Mnem.ADDSUB_InnnI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();

                mem.Set8bitToRAM(
                    target,
                    regs.Add8BitValues(
                        mem.Get8bitFromRAM(target),
                        mem.Fetch()
                    ));
            };

            table[Mnem.ADDSUB16_InnnI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(
                        mem.Get16bitFromRAM(target),
                        mem.Fetch16()
                    ));
            };

            table[Mnem.ADDSUB24_InnnI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        mem.Fetch24()
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        mem.Fetch32()
                    ));
            };

            table[Mnem.ADDSUB_InnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set8bitToRAM(
                    target,
                    regs.Add8BitValues(
                        mem.Get8bitFromRAM(target),
                        regs.Get8BitRegister(regIndex)
                    ));
            };

            table[Mnem.ADDSUB16_InnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(
                        mem.Get16bitFromRAM(target),
                        regs.Get8BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB24_InnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        regs.Get8BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        regs.Get8BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB16_InnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(
                        mem.Get16bitFromRAM(target),
                        regs.Get16BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB24_InnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        regs.Get16BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = (byte)(mem.Fetch() & 0b00011111);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        regs.Get16BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB24_InnnI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = mem.Fetch();

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        regs.Get24BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = mem.Fetch();

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        regs.Get24BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                byte regIndex = mem.Fetch();

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        regs.Get32BitRegister((byte)(regIndex & 0b00011111))
                    ));
            };

            // -----------------------
            // ADD to memory (register-indirect)
            // -----------------------

            table[Mnem.ADDSUB_IrrrI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));

                mem.Set8bitToRAM(
                    target,
                    regs.Add8BitValues(
                        mem.Get8bitFromRAM(target),
                        mem.Fetch()
                    ));
            };

            table[Mnem.ADDSUB16_IrrrI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(
                        mem.Get16bitFromRAM(target),
                        mem.Fetch16()
                    ));
            };

            table[Mnem.ADDSUB24_IrrrI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        mem.Fetch24()
                    ));
            };

            table[Mnem.ADDSUB32_IrrrI_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        mem.Fetch32()
                    ));
            };

            table[Mnem.ADDSUB_IrrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                byte regValue = regs.Get8BitRegister((byte)(mem.Fetch() & 0b00011111));
                byte memValue = mem.Get8bitFromRAM(target);

                mem.Set8bitToRAM(
                    target,
                    regs.Add8BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB16_IrrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                byte regValue = regs.Get8BitRegister((byte)(mem.Fetch() & 0b00011111));
                ushort memValue = mem.Get16bitFromRAM(target);

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB24_IrrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                byte regValue = regs.Get8BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get24bitFromRAM(target);

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB32_IrrrI_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                byte regValue = regs.Get8BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get32bitFromRAM(target);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB16_IrrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                ushort regValue = regs.Get16BitRegister((byte)(mem.Fetch() & 0b00011111));
                ushort memValue = mem.Get16bitFromRAM(target);

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB24_IrrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                ushort regValue = regs.Get16BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get24bitFromRAM(target);

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB32_IrrrI_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                ushort regValue = regs.Get16BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get32bitFromRAM(target);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB24_IrrrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint regValue = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get24bitFromRAM(target);

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB32_IrrrI_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint regValue = regs.Get24BitRegister((byte)(mem.Fetch() & 0b00011111));
                uint memValue = mem.Get32bitFromRAM(target);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(memValue, regValue));
            };

            table[Mnem.ADDSUB_IrrrI_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = regs.Get24BitRegister(mem.Fetch());
                uint regValue = regs.Get32BitRegister(mem.Fetch());
                uint memValue = mem.Get32bitFromRAM(target);

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(memValue, regValue));
            };

            // -----------------------
            // ADD memory + memory
            // -----------------------

            table[Mnem.ADDSUB_InnnI_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint secondValue = mem.Fetch24();

                mem.Set8bitToRAM(
                    target,
                    regs.Add8BitValues(
                        mem.Get8bitFromRAM(target),
                        mem.Get8bitFromRAM(secondValue)
                    ));
            };

            table[Mnem.ADDSUB16_InnnI_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint secondValue = mem.Fetch24();

                mem.Set16bitToRAM(
                    target,
                    regs.Add16BitValues(
                        mem.Get16bitFromRAM(target),
                        mem.Get16bitFromRAM(secondValue)
                    ));
            };

            table[Mnem.ADDSUB24_InnnI_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint secondValue = mem.Fetch24();

                mem.Set24bitToRAM(
                    target,
                    regs.Add24BitValues(
                        mem.Get24bitFromRAM(target),
                        mem.Get24bitFromRAM(secondValue)
                    ));
            };

            table[Mnem.ADDSUB32_InnnI_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;

                uint target = mem.Fetch24();
                uint secondValue = mem.Fetch24();

                mem.Set32bitToRAM(
                    target,
                    regs.Add32BitValues(
                        mem.Get32bitFromRAM(target),
                        mem.Get32bitFromRAM(secondValue)
                    ));
            };

            // -----------------------
            // Floating point ADD
            // -----------------------

            table[Mnem.ADDSUB_fr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte fReg1 = (byte)(mem.Fetch() & 0b00001111);
                byte fReg2 = (byte)(mem.Fetch() & 0b00001111);

                float fReg1Value = fregs.GetRegister(fReg1);
                float fReg2Value = fregs.GetRegister(fReg2);

                fregs.SetRegister(
                    fReg1,
                    fregs.AddFloatValues(fReg1Value, fReg2Value));
            };

            table[Mnem.ADDSUB_fr_nnn] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte fReg = (byte)(mem.Fetch() & 0b00001111);
                uint floatBitValue = mem.Fetch32();
                float floatValue = FloatPointUtils.UintToFloat(floatBitValue);
                float fRegValue = fregs.GetRegister(fReg);

                fregs.SetRegister(
                    fReg,
                    fregs.AddFloatValues(fRegValue, floatValue));
            };

            table[Mnem.ADDSUB_fr_r] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);
                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);

                float regValue = regs.Get8BitRegister(regIndex);
                float fRegValue = fregs.GetRegister(fRegIndex);

                fregs.SetRegister(
                    fRegIndex,
                    fregs.AddFloatValues(fRegValue, regValue));
            };

            table[Mnem.ADDSUB_fr_rr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);
                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);

                float regValue = regs.Get16BitRegister(regIndex);
                float fRegValue = fregs.GetRegister(fRegIndex);

                fregs.SetRegister(
                    fRegIndex,
                    fregs.AddFloatValues(fRegValue, regValue));
            };

            table[Mnem.ADDSUB_fr_rrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);
                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);

                float regValue = regs.Get24BitRegister(regIndex);
                float fRegValue = fregs.GetRegister(fRegIndex);

                fregs.SetRegister(
                    fRegIndex,
                    fregs.AddFloatValues(fRegValue, regValue));
            };

            table[Mnem.ADDSUB_fr_rrrr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);
                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);

                float regValue = regs.Get32BitRegister(regIndex);
                float fRegValue = fregs.GetRegister(fRegIndex);

                fregs.SetRegister(
                    fRegIndex,
                    fregs.AddFloatValues(fRegValue, regValue));
            };

            // float -> integer ADD (using sign to choose add vs sub)

            table[Mnem.ADDSUB_r_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);
                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);

                float fRegValue = fregs.GetRegister(fRegIndex);
                bool isNegative = fRegValue < 0;
                fRegValue = Math.Abs(fRegValue);

                byte fConverted;
                if (fRegValue > 0xFF)
                {
                    fConverted = (byte)Math.Round(fRegValue % 0x100);
                    flags.SetOverflow(true);
                }
                else
                {
                    fConverted = (byte)Math.Round(fRegValue);
                    flags.SetOverflow(false);
                }

                if (isNegative)
                {
                    regs.SubtractFrom8BitRegister(regIndex, fConverted);
                }
                else
                {
                    regs.AddTo8BitRegister(regIndex, fConverted);
                }
            };

            table[Mnem.ADDSUB_rr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);
                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);

                float fRegValue = fregs.GetRegister(fRegIndex);
                bool isNegative = fRegValue < 0;
                fRegValue = Math.Abs(fRegValue);

                ushort fConverted;
                if (fRegValue > 0xFFFF)
                {
                    fConverted = (ushort)Math.Round(fRegValue % 0x10000);
                    flags.SetOverflow(true);
                }
                else
                {
                    fConverted = (ushort)Math.Round(fRegValue);
                    flags.SetOverflow(false);
                }

                if (isNegative)
                {
                    regs.SubtractFrom16BitRegister(regIndex, fConverted);
                }
                else
                {
                    regs.AddTo16BitRegister(regIndex, fConverted);
                }
            };

            table[Mnem.ADDSUB_rrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);
                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);

                float fRegValue = fregs.GetRegister(fRegIndex);
                bool isNegative = fRegValue < 0;
                fRegValue = Math.Abs(fRegValue);

                uint fConverted;
                if (fRegValue > 0xFFFFFF)
                {
                    fConverted = (uint)Math.Round(fRegValue % 0x1000000);
                    flags.SetOverflow(true);
                }
                else
                {
                    fConverted = (uint)Math.Round(fRegValue);
                    flags.SetOverflow(false);
                }

                if (isNegative)
                {
                    regs.SubtractFrom24BitRegister(regIndex, fConverted);
                }
                else
                {
                    regs.AddTo24BitRegister(regIndex, fConverted);
                }
            };

            table[Mnem.ADDSUB_rrrr_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;
                var flags = cpu.CPU.FLAGS;

                byte regIndex = (byte)(mem.Fetch() & 0b_00011111);
                byte fRegIndex = (byte)(mem.Fetch() & 0b_00001111);

                float fRegValue = fregs.GetRegister(fRegIndex);
                bool isNegative = fRegValue < 0;
                fRegValue = Math.Abs(fRegValue);

                uint fConverted;
                if (fRegValue > 0xFFFFFFFF)
                {
                    fConverted = (uint)Math.Round(fRegValue % 0x100000000);
                    flags.SetOverflow(true);
                }
                else
                {
                    fConverted = (uint)Math.Round(fRegValue);
                    flags.SetOverflow(false);
                }

                if (isNegative)
                {
                    regs.SubtractFrom32BitRegister(regIndex, fConverted);
                }
                else
                {
                    regs.AddTo32BitRegister(regIndex, fConverted);
                }
            };

            table[Mnem.ADDSUB_fr_InnnI] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte fReg = (byte)(mem.Fetch() & 0b00001111);
                uint adrFloatPointer = mem.Fetch24();
                float adrFloatValue = mem.GetFloatFromRAM(adrFloatPointer);
                float fRegValue = fregs.GetRegister(fReg);

                fregs.SetRegister(
                    fReg,
                    fregs.AddFloatValues(fRegValue, adrFloatValue));
            };

            table[Mnem.ADDSUB_fr_IrrrI] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegPointer = (byte)(mem.Fetch() & 0b00001111);
                byte regPointer = (byte)(mem.Fetch() & 0b00011111);

                uint adrFloatPointer = regs.Get24BitRegister(regPointer);

                float adrFloatValue = mem.GetFloatFromRAM(adrFloatPointer);
                float fRegValue = fregs.GetRegister(fRegPointer);

                fregs.SetRegister(
                    fRegPointer,
                    fregs.AddFloatValues(fRegValue, adrFloatValue));
            };

            table[Mnem.ADDSUB_InnnI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var fregs = cpu.CPU.FREGS;

                byte fReg = (byte)(mem.Fetch() & 0b00001111);
                uint adrFloatPointer = mem.Fetch24();
                float adrFloatValue = mem.GetFloatFromRAM(adrFloatPointer);
                float fRegValue = fregs.GetRegister(fReg);

                mem.SetFloatToRam(
                    adrFloatPointer,
                    fregs.AddFloatValues(fRegValue, adrFloatValue));
            };

            table[Mnem.ADDSUB_IrrrI_fr] = cpu =>
            {
                var mem = cpu.MEMC;
                var regs = cpu.CPU.REGS;
                var fregs = cpu.CPU.FREGS;

                byte fRegPointer = (byte)(mem.Fetch() & 0b00001111);
                byte regPointer = (byte)(mem.Fetch() & 0b00011111);

                uint adrFloatPointer = regs.Get24BitRegister(regPointer);

                float adrFloatValue = mem.GetFloatFromRAM(adrFloatPointer);
                float fRegValue = fregs.GetRegister(fRegPointer);

                mem.SetFloatToRam(
                    adrFloatPointer,
                    fregs.AddFloatValues(fRegValue, adrFloatValue));
            };

            return table;
        }

        public static void Process(Computer computer)
        {
            _dispatch[computer.MEMC.Fetch()](computer);
        }
    }
}
