using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;

namespace Continuum93.Emulator.Execution
{
    public class ExPUSH
    {
        private static Computer _computer;

        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            _computer = computer;

            switch (ldOp)
            {
                case Mnem.POPU_r:   // PUSH r
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        //if (CheckAndHandleStackOverflow(1, $"push register {Constants.ALPHABET.Substring(regIndex, 1)}")) return;

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 1)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register {Constants.ALPHABET.Substring(regIndex, 1)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        byte regValue = _computer.CPU.REGS.Get8BitRegister(regIndex);
                        _computer.MEMC.Set8BitToRegStack(_computer.CPU.REGS.SPR++, regValue);

                        return;
                    }
                case Mnem.POPU_rr:   // PUSH rr
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        
                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 2)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register {Constants.ALPHABET.Substring(regIndex, 2)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        ushort regValue = _computer.CPU.REGS.Get16BitRegister(regIndex);
                        _computer.MEMC.Set16BitToRegStack(_computer.CPU.REGS.SPR, regValue);
                        _computer.CPU.REGS.SPR += 2;

                        return;
                    }
                case Mnem.POPU_rrr:   // PUSH rrr
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 3)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint regValue = _computer.CPU.REGS.Get24BitRegister(regIndex);
                        _computer.MEMC.Set24BitToRegStack(_computer.CPU.REGS.SPR, regValue);
                        _computer.CPU.REGS.SPR += 3;

                        return;
                    }
                case Mnem.POPU_rrrr:   // PUSH rrrr
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 4)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register {Constants.ALPHABET.Substring(regIndex, 4)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint regValue = _computer.CPU.REGS.Get32BitRegister(regIndex);
                        _computer.MEMC.Set32BitToRegStack(_computer.CPU.REGS.SPR, regValue);
                        _computer.CPU.REGS.SPR += 4;

                        return;
                    }
                case Mnem.POPU_r_r:     // PUSH r, r
                    {
                        byte reg1Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte distance = CalculateRegisterDistance(reg1Index, reg2Index);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - distance)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register range {Constants.ALPHABET.Substring(reg1Index, 1)}, {Constants.ALPHABET.Substring(reg2Index, 1)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        while (reg1Index != reg2Index)
                        {
                            Push8BitReg(reg1Index);
                            reg1Index = (reg1Index == Mnem.Z) ? Mnem.A : (byte)(reg1Index + 1);
                        }

                        Push8BitReg(reg1Index);

                        return;
                    }
                case Mnem.POPU_fr:      // PUSH fr
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 4)
                        {
                            Log.WriteLine($"Stack overflow when trying to push float register F{fRegIndex}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        PushFloatReg(fRegIndex);
                        return;
                    }
                case Mnem.POPU_fr_fr:   // PUSH fr, fr
                    {
                        byte mixReg = computer.MEMC.Fetch();
                        byte fReg1Index = (byte)(mixReg & 0b_11110000);
                        byte fReg2Index = (byte)(mixReg & 0b_00001111);

                        byte distance = CalculateRegisterDistance(fReg1Index, fReg2Index);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - distance * 4)
                        {
                            Log.WriteLine($"Stack overflow when trying to push register range F{fReg1Index}, F{fReg2Index}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        while (fReg1Index != fReg2Index)
                        {
                            PushFloatReg(fReg1Index);
                            fReg1Index = (byte)((fReg1Index == 15) ? 0 : fReg1Index + 1);
                        }

                        PushFloatReg(fReg1Index);

                        return;
                    }
                case Mnem.POPU_InnnI: // PUSH (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 1)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        byte value = _computer.MEMC.Get8bitFromRAM(address);
                        _computer.MEMC.Set8BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 1;

                        return;
                    }
                case Mnem.POPU_IrrrI: // PUSH (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 1)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        byte value = _computer.MEMC.Get8bitFromRAM(address);
                        _computer.MEMC.Set8BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 1;

                        return;
                    }
                case Mnem.POPU16_InnnI: // PUSH16 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 2)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        ushort value = _computer.MEMC.Get16bitFromRAM(address);
                        _computer.MEMC.Set16BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 2;

                        return;
                    }
                case Mnem.POPU16_IrrrI: // PUSH16 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 2)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        ushort value = _computer.MEMC.Get16bitFromRAM(address);
                        _computer.MEMC.Set16BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 2;

                        return;
                    }
                case Mnem.POPU24_InnnI: // PUSH24 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 3)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint value = _computer.MEMC.Get24bitFromRAM(address);
                        _computer.MEMC.Set24BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 3;

                        return;
                    }
                case Mnem.POPU24_IrrrI: // PUSH24 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 3)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint value = _computer.MEMC.Get24bitFromRAM(address);
                        _computer.MEMC.Set24BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 3;

                        return;
                    }
                case Mnem.POPU32_InnnI: // PUSH32 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 4)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint value = _computer.MEMC.Get32bitFromRAM(address);
                        _computer.MEMC.Set32BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 4;

                        return;
                    }
                case Mnem.POPU32_IrrrI: // PUSH32 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - 4)
                        {
                            Log.WriteLine($"Stack overflow when trying to push value from address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            computer.MEMC.ResetAllStacks();
                            computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
                            return;
                        }

                        uint value = _computer.MEMC.Get32bitFromRAM(address);
                        _computer.MEMC.Set32BitToRegStack(_computer.CPU.REGS.SPR, value);
                        _computer.CPU.REGS.SPR += 4;

                        return;
                    }
            }
        }

        private static void Push8BitReg(byte regIndex)
        {
            byte regValue = _computer.CPU.REGS.Get8BitRegister(regIndex);
            _computer.MEMC.Set8BitToRegStack(_computer.CPU.REGS.SPR++, regValue);
        }

        private static void PushFloatReg(byte fRegIndex)
        {
            float floatValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
            uint floatUintValue = FloatPointUtils.FloatToUint(floatValue);
            _computer.MEMC.Set32BitToRegStack(_computer.CPU.REGS.SPR, floatUintValue);
            _computer.CPU.REGS.SPR += 4;
        }

        private static byte CalculateRegisterDistance(byte start, byte end)
        {
            return (byte)(((end - start + 26) % 26) + 1);
        }

        private static byte CalculateFloatRegisterDistance(byte start, byte end)
        {
            return (byte)(((end - start + 16) % 16) + 1);
        }

        private static bool CheckAndHandleStackOverflow(int requiredSpace, string description)
        {
            if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - requiredSpace)
            {
                Log.WriteLine($"Stack overflow when trying to {description}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                _computer.MEMC.ResetAllStacks();
                _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                return true;
            }
            return false;
        }
    }
}
