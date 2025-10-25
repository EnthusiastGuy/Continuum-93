using Continuum93.Emulator.Controls;
using Continuum93.Emulator.Window;
using Continuum93.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsInput
    {
        public static void ReadKeyboardStateAsBits(byte regId, Computer computer)
        {
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to
            //byte[] keys = InputKeyboard.GetPressedKeysFast();
            //byte[] keys = InputKeyboard.GetPressedKeys();
            byte[] keysState = KeyboardStateExt.GetStateAsBytes();

            computer.LoadMemAt(destAdr, keysState);
            computer.CPU.REGS.Set8BitRegister(regId, (byte)keysState.Length);
        }

        public static void ReadKeyboardStateAsBytes(byte regId, Computer computer)
        {
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);       // Get the pointer to the start memory address to load the file to
            byte[] keysState = KeyboardStateExt.GetStateAsCodes();

            computer.LoadMemAt(destAdr, keysState);
            computer.CPU.REGS.Set8BitRegister(regId, (byte)keysState.Length);
        }

        public static void TriggerKeyboardHandler(char c, Keys key, Computer computer)
        {
            //Debug.WriteLine("Typed " + c);
            InterruptCallbacks.CharBuffer.Add(c);
            if (InterruptCallbacks.KeyboardCallback != 0xFFFFFF)
            {
                InterruptCallbacks.CharBuffer.Add(c);

                computer.Pause();
                computer.MEMC.Set24BitToRegStack(computer.CPU.REGS.SPR, computer.CPU.REGS.IPO);
                computer.CPU.REGS.SPR += 3;
                computer.CPU.REGS.IPO = InterruptCallbacks.KeyboardCallback;
                computer.Unpause();
            }

            if (c >= 32 && c <= 126)    //&& !(KeyboardStates.KeyDown(Keys.LeftControl) || KeyboardStates .KeyDown(Keys.RightControl))
            {
                //Terminal.SendCharacter(c);
            }
            else if (c == 8)
            {
                //Terminal.BackspaceCharacterAtCaret();
            }
        }

        public static void ReadKeyboardBuffer(byte regId, Computer computer)
        {
            byte statReg = computer.CPU.REGS.GetNextRegister(regId, 1);

            if (InterruptCallbacks.CharBuffer.Count == 0)
            {
                computer.CPU.REGS.Set8BitRegister(regId, 0);
                computer.CPU.REGS.Set8BitRegister(statReg, 0);
            }
            else if (InterruptCallbacks.CharBuffer.Count == 1)
            {
                computer.CPU.REGS.Set8BitRegister(regId, Convert.ToByte(InterruptCallbacks.CharBuffer[0]));
                computer.CPU.REGS.Set8BitRegister(statReg, 0);
                InterruptCallbacks.CharBuffer.Clear();
            }
            else if (InterruptCallbacks.CharBuffer.Count < 255)
            {
                computer.CPU.REGS.Set8BitRegister(regId, Convert.ToByte(InterruptCallbacks.CharBuffer[0]));
                InterruptCallbacks.CharBuffer.RemoveAt(0);
                computer.CPU.REGS.Set8BitRegister(statReg, (byte)InterruptCallbacks.CharBuffer.Count);
            }
            else
            {
                // Possible overflow state
                computer.CPU.REGS.Set8BitRegister(regId, Convert.ToByte(InterruptCallbacks.CharBuffer[0]));
                computer.CPU.REGS.Set8BitRegister(statReg, (byte)InterruptCallbacks.CharBuffer.Count);
                InterruptCallbacks.CharBuffer.RemoveAt(0);
            }
        }

        /// <summary>
        /// Provides the address to where any keyboard state change should be handled.
        /// If this address is 0xFFFFFF it means the callback is disabled.
        /// </summary>
        /// <param name="regId"></param>
        public static void HandleKeyboardStateChanged(byte regId, Computer computer)
        {
            InterruptCallbacks.KeyboardCallback = computer.CPU.REGS.Get24BitRegister(
                computer.CPU.REGS.GetNextRegister(regId, 1));
        }

        public static void HandleControllerStateChanged(byte regId, Computer computer)
        {
            InterruptCallbacks.ControllerCallback = computer.CPU.REGS.Get24BitRegister(
                computer.CPU.REGS.GetNextRegister(regId, 1));
        }

        public static void ReadMouseState(byte regId, Computer computer)
        {
            byte rMX = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte rMY = computer.CPU.REGS.GetNextRegister(regId, 3);
            byte rMS = computer.CPU.REGS.GetNextRegister(regId, 5);

            Point xy = InputMouse.GetRelativeMousePos();

            computer.CPU.REGS.Set16BitRegister(rMX, (ushort)(xy.X / WindowManager.GetClientXRatio()));
            computer.CPU.REGS.Set16BitRegister(rMY, (ushort)(xy.Y / WindowManager.GetClientYRatio()));

            // Mouse state returns buttons as such:
            // bit 7 ------ 6 ------ 5 ------ 4 ------ 3 ------ 2 ------ 1 ------ 0
            //    N.A.     N.A.     N.A.     N.A.     N.A.   MBUTTON  RBUTTON  LBUTTON
            computer.CPU.REGS.Set8BitRegister(rMS, InputMouse.GetMouseButtonsState());
        }

        public static void HandleMouseStateChanged(byte regId, Computer computer)
        {
            InterruptCallbacks.MouseCallback = computer.CPU.REGS.Get24BitRegister(
                computer.CPU.REGS.GetNextRegister(regId, 1));
        }

        public static void ReadGamePadsState(byte regId, Computer computer)
        {
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);
            byte[] gamepadsState = GamepadStateExts.GetGamepadsState();

            computer.LoadMemAt(destAdr, gamepadsState);
        }

        public static void ReadGamePadsCapabilities(byte regId, Computer computer)
        {
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);
            byte[] gamepadsState = GamepadStateExts.GetGamepadsCapabilities();

            computer.LoadMemAt(destAdr, gamepadsState);
        }

        public static void ReadGamePadsNames(byte regId, Computer computer)
        {
            byte destAdrReg = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint destAdr = computer.CPU.REGS.Get24BitRegister(destAdrReg);
            byte[] gamepadsState = GamepadStateExts.GetGamepadDisplayNames();

            computer.CPU.REGS.Set8BitRegister(regId, (byte)gamepadsState.Length);

            computer.LoadMemAt(destAdr, gamepadsState);
        }
    }
}
