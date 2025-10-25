using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System.Net;

namespace Continuum93.Emulator.Execution
{
    public class ExPOP
    {
        private static Computer _computer;
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            _computer = computer;

            switch (ldOp)
            {
                case Mnem.POPU_r:   // POP r
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        Pop8BitReg(regIndex);

                        return;
                    }
                case Mnem.POPU_rr:   // POP rr
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        if (_computer.CPU.REGS.SPR < 2)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop register {Constants.ALPHABET.Substring(regIndex, 2)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 2;
                        ushort stackVal = _computer.MEMC.Get16BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.CPU.REGS.Set16BitRegister(regIndex, stackVal);

                        return;
                    }
                case Mnem.POPU_rrr:   // POP rrr
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        if (_computer.CPU.REGS.SPR < 3)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 3;
                        uint stackVal = _computer.MEMC.Get24BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.CPU.REGS.Set24BitRegister(regIndex, stackVal);

                        return;
                    }
                case Mnem.POPU_rrrr:   // POP rrr
                    {
                        byte regIndex = computer.MEMC.Fetch();

                        if (_computer.CPU.REGS.SPR < 4)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop register {Constants.ALPHABET.Substring(regIndex, 4)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 4;
                        uint stackVal = _computer.MEMC.Get32BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.CPU.REGS.Set32BitRegister(regIndex, stackVal);

                        return;
                    }
                case Mnem.POPU_r_r:     // POP r, r
                    {
                        byte reg1Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        while (reg1Index != reg2Index)
                        {
                            Pop8BitReg(reg2Index);
                            reg2Index = (reg2Index == Mnem.A) ? Mnem.Z : (byte)(reg2Index - 1);
                        }

                        Pop8BitReg(reg2Index);

                        return;
                    }
                case Mnem.POPU_fr:      // POP fr
                    {
                        byte fRegIndex = computer.MEMC.Fetch();
                        PopFloatReg(fRegIndex);
                        return;
                    }
                case Mnem.POPU_fr_fr:   // POP fr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg1Index = (byte)(mixedReg >> 4);
                        byte fReg2Index = (byte)(mixedReg & 0b_00001111);

                        while (fReg1Index != fReg2Index)
                        {
                            PopFloatReg(fReg2Index);
                            fReg2Index = (byte)((fReg2Index == 0) ? 15 : fReg2Index - 1);
                        }

                        PopFloatReg(fReg2Index);

                        return;
                    }
                case Mnem.POPU_InnnI:   // POP (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();

                        if (_computer.CPU.REGS.SPR < 1)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR--;
                        byte stackVal = _computer.MEMC.Get8BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set8bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU_IrrrI:   // POP (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR < 1)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address} pointed by register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR--;
                        byte stackVal = _computer.MEMC.Get8BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set8bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU16_InnnI:   // POP16 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();

                        if (_computer.CPU.REGS.SPR < 2)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 2;
                        ushort stackVal = _computer.MEMC.Get16BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set16bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU16_IrrrI:   // POP16 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR < 2)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address} pointed by register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 2;
                        ushort stackVal = _computer.MEMC.Get16BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set16bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU24_InnnI:   // POP24 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();

                        if (_computer.CPU.REGS.SPR < 3)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 3;
                        uint stackVal = _computer.MEMC.Get24BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set24bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU24_IrrrI:   // POP24 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR < 3)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address} pointed by register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 3;
                        uint stackVal = _computer.MEMC.Get24BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set24bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU32_InnnI:   // POP32 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();

                        if (_computer.CPU.REGS.SPR < 4)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 4;
                        uint stackVal = _computer.MEMC.Get32BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set32bitToRAM(address, stackVal);

                        return;
                    }
                case Mnem.POPU32_IrrrI:   // POP32 (rrr)
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (_computer.CPU.REGS.SPR < 4)
                        {
                            Log.WriteLine($"Stack underflow when trying to pop to address {address} pointed by register {Constants.ALPHABET.Substring(regIndex, 3)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                            _computer.MEMC.ResetAllStacks();
                            _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                            _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                            return;
                        }

                        _computer.CPU.REGS.SPR -= 4;
                        uint stackVal = _computer.MEMC.Get32BitFromRegStack(_computer.CPU.REGS.SPR);
                        _computer.MEMC.Set32bitToRAM(address, stackVal);

                        return;
                    }
            }
        }

        private static void Pop8BitReg(byte regIndex)
        {
            if (_computer.CPU.REGS.SPR < 1)
            {
                Log.WriteLine($"Stack underflow when trying to pop register {Constants.ALPHABET.Substring(regIndex, 1)}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                _computer.MEMC.ResetAllStacks();
                _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                return;
            }

            _computer.CPU.REGS.SPR--;
            byte stackVal = _computer.MEMC.Get8BitFromRegStack(_computer.CPU.REGS.SPR);
            _computer.CPU.REGS.Set8BitRegister(regIndex, stackVal);
        }

        private static void PopFloatReg(byte fRegIndex)
        {
            if (_computer.CPU.REGS.SPR >= 4)
            {
                _computer.CPU.REGS.SPR -= 4;
                uint stackVal = _computer.MEMC.Get32BitFromRegStack(_computer.CPU.REGS.SPR);
                float floatValue = FloatPointUtils.UintToFloat(stackVal);
                _computer.CPU.FREGS.SetRegister(fRegIndex, floatValue);
            }
            else
            {
                Log.WriteLine($"Stack underflow when trying to pop float register F{fRegIndex}. Instruction pointer: {_computer.CPU.REGS.IPO}");
                _computer.MEMC.ResetAllStacks();
                _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
                _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
                return;
            }
        }
    }
}
