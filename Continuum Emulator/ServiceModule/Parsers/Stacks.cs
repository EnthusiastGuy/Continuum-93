using Continuum93.Emulator;
using Continuum93.Emulator.RAM;
using System;
using System.Collections.Generic;

namespace Continuum93.ServiceModule.Parsers
{
    public static class Stacks
    {
        public static List<string> RegisterStack = new();
        public static List<string> CallStack = new();

        public static void Update()
        {
            RegisterStack.Clear();
            CallStack.Clear();

            if (Machine.COMPUTER == null)
                return;

            var cpu = Machine.COMPUTER.CPU;
            var memc = Machine.COMPUTER.MEMC;

            // Register stack
            uint regStackPointer = cpu.REGS.SPR;
            uint regStackLimit = Math.Min(regStackPointer, 120);
            for (uint i = 0; i < regStackLimit; i++)
            {
                byte value = memc.Get8BitFromRegStack(regStackPointer - regStackLimit + i);
                RegisterStack.Add(value.ToString("X2"));
            }

            // Call stack
            uint callStackPointer = cpu.REGS.SPC;
            uint callStackLimit = Math.Min(callStackPointer, 64);
            for (uint i = 0; i < callStackLimit; i++)
            {
                uint value = memc.GetFromCallStack(callStackPointer - callStackLimit + i);
                CallStack.Add(value.ToString("X6"));
            }
        }
    }
}






