/*
 * InstructionSet Class
 * ----------------------
 * 
 * This static class constructs and exposes a jump table (IJT) for the emulator. The jump table is a static 
 * array of delegate actions (Action<Computer>[]) with 256 entries, one for each possible opcode (0–255).
 * 
 * The primary purpose of this table is to provide a fast, direct dispatch mechanism for executing CPU instructions.
 * When the Computer instance fetches an opcode, it looks up the corresponding delegate in this table and executes it.
 * This design avoids the overhead of lengthy switch-case or if-else chains in the inner emulation loop.
 * 
 * Key points:
 *   - The table is initialized once in the static constructor and remains immutable thereafter, ensuring fast access.
 *   - Each opcode is mapped to its corresponding processing method (e.g., ExLD.Process for the load instruction, 
 *     ExADD.Process for addition, etc.). If an opcode isn’t implemented, its entry will remain null.
 *   - The opcodes are defined in the Mnem class, which serves as a central reference for instruction codes.
 *   - Since the jump table is static and read-only after initialization, it minimizes runtime overhead and aids in 
 *     achieving efficient instruction dispatch in the emulator's core execution loop.
 * 
 * To extend or modify instruction handling, update the corresponding entry in the static constructor.
 */
using Continuum93.Emulator.Execution;
using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator
{
    public static class InstructionSet
    {
        public static readonly Action<Computer>[] IJT = new Action<Computer>[256];
        static void NoOp(Computer c) { /* nothing */ }

        static InstructionSet()
        {
            for (int i = 0; i < IJT.Length; i++)
                IJT[i] = NoOp;

            IJT[Mnem.NOOP] = _ => { /* Shrug */ };
            IJT[Mnem.LD] = ExLD.Process;
            IJT[Mnem.ADD] = ExADD.Process;
            IJT[Mnem.SUB] = ExSUB.Process;
            IJT[Mnem.DIV] = ExDIV.Process;
            IJT[Mnem.MOD] = ExMOD.Process;
            IJT[Mnem.MUL] = ExMUL.Process;
            IJT[Mnem.SL] = ExSL.Process;
            IJT[Mnem.SR] = ExSR.Process;
            IJT[Mnem.RL] = ExRL.Process;
            IJT[Mnem.RR] = ExRR.Process;
            IJT[Mnem.SET] = ExSET.Process;
            IJT[Mnem.RES] = ExRES.Process;
            IJT[Mnem.BIT] = ExBIT.Process;
            IJT[Mnem.AND] = ExAND.Process;
            IJT[Mnem.OR] = ExOR.Process;
            IJT[Mnem.XOR] = ExXOR.Process;
            IJT[Mnem.INV] = ExINV.Process;
            IJT[Mnem.EX] = ExEX.Process;
            IJT[Mnem.CP] = ExCP.Process;
            IJT[Mnem.INC] = ExINC.Process;
            IJT[Mnem.DEC] = ExDEC.Process;
            IJT[Mnem.CALL] = ExCALL.Process;
            IJT[Mnem.CALLR] = ExCALLR.Process;
            IJT[Mnem.JP] = ExJP.Process;
            IJT[Mnem.JR] = ExJR.Process;
            IJT[Mnem.POP] = ExPOP.Process;
            IJT[Mnem.PUSH] = ExPUSH.Process;
            IJT[Mnem.INT] = ExINT.Process;
            IJT[Mnem.RAND] = ExRAND.Process;
            IJT[Mnem.MIN] = ExMIN.Process;
            IJT[Mnem.MAX] = ExMAX.Process;
            IJT[Mnem.DJNZ] = ExDJNZ.Process;
            IJT[Mnem.SETF] = ExSETF.Process;
            IJT[Mnem.RESF] = ExRESF.Process;
            IJT[Mnem.INVF] = ExINVF.Process;
            IJT[Mnem.LDF] = ExLDF.Process;
            IJT[Mnem.REGS] = ExREGS.Process;
            IJT[Mnem.WAIT] = ExWAIT.Process;
            IJT[Mnem.VDL] = ExVDL.Process;
            IJT[Mnem.VCL] = ExVDL.Process;
            IJT[Mnem.FIND] = ExFIND.Process;
            IJT[Mnem.IMPLY] = ExIMPLY.Process;
            IJT[Mnem.NAND] = ExNAND.Process;
            IJT[Mnem.NOR] = ExNOR.Process;
            IJT[Mnem.XNOR] = ExXNOR.Process;
            IJT[Mnem.GETBITS] = ExGETBITS.Process;
            IJT[Mnem.SETBITS] = ExSETBITS.Process;
            IJT[Mnem.MEMF] = ExMEMF.Process;
            IJT[Mnem.MEMC] = ExMEMC.Process;
            IJT[Mnem.LDREGS] = ExLDREGS.Process;
            IJT[Mnem.STREGS] = ExSTREGS.Process;
            IJT[Mnem.SDIV] = ExSDIV.Process;
            IJT[Mnem.SMUL] = ExSMUL.Process;
            IJT[Mnem.SCP] = ExSCP.Process;
            IJT[Mnem.FREGS] = ExFREGS.Process;
            IJT[Mnem.POW] = ExPOW.Process;
            IJT[Mnem.SQR] = ExSQR.Process;
            IJT[Mnem.CBR] = ExCBR.Process;
            IJT[Mnem.ISQR] = ExISQR.Process;
            IJT[Mnem.ISGN] = ExISGN.Process;
            IJT[Mnem.SIN] = ExSIN.Process;
            IJT[Mnem.COS] = ExCOS.Process;
            IJT[Mnem.TAN] = ExTAN.Process;
            IJT[Mnem.ABS] = ExABS.Process;
            IJT[Mnem.ROUND] = ExROUND.Process;
            IJT[Mnem.FLOOR] = ExFLOOR.Process;
            IJT[Mnem.CEIL] = ExCEIL.Process;
            IJT[Mnem.PLAY] = ExPLAY.Process;
            IJT[Mnem.SETVAR] = ExSETVAR.Process;
            IJT[Mnem.GETVAR] = ExGETVAR.Process;
            IJT[Mnem.RGB2HSL] = ExC_RGB2HSL.Process;
            IJT[Mnem.HSL2RGB] = ExC_HSL2RGB.Process;
            IJT[Mnem.RGB2HSB] = ExC_RGB2HSB.Process;
            IJT[Mnem.HSB2RGB] = ExC_HSB2RGB.Process;
            IJT[Mnem.DEBUG] = ExDEBUG.Process;
            IJT[Mnem.BREAK] = ExMisc.ProcessBreak;
            IJT[Mnem.RETIF] = ExRETIF.Process;
            IJT[Mnem.RET] = ExMisc.ProcessRET;
        }
    }
}
