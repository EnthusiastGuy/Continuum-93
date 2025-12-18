# Continuum 93 Instruction Documentation Index

This document provides an index of all instructions in the Continuum 93 instruction set and their documentation status.

## Documentation Status

- ✅ **Documented**: Complete analysis document available
- 📝 **In Progress**: Documentation being created
- ⏳ **Pending**: Documentation needed
- 🔍 **Review Needed**: Implementation needs review before documentation

## Core Arithmetic Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| LD | ExLD.cs | ✅ | [LD_INSTRUCTION_ANALYSIS.md](LD_INSTRUCTION_ANALYSIS.md) |
| ADD | ExADD.cs | ✅ | [ADD_INSTRUCTION_ANALYSIS.md](ADD_INSTRUCTION_ANALYSIS.md) |
| SUB | ExSUB.cs | ✅ | [SUB_INSTRUCTION_ANALYSIS.md](SUB_INSTRUCTION_ANALYSIS.md) |
| MUL | ExMUL.cs | ✅ | [MUL_INSTRUCTION_ANALYSIS.md](MUL_INSTRUCTION_ANALYSIS.md) |
| DIV | ExDIV.cs | ✅ | [DIV_INSTRUCTION_ANALYSIS.md](DIV_INSTRUCTION_ANALYSIS.md) |
| SDIV | ExSDIV.cs | ✅ | [SDIV_INSTRUCTION_ANALYSIS.md](SDIV_INSTRUCTION_ANALYSIS.md) |
| SMUL | ExSMUL.cs | ✅ | [SMUL_INSTRUCTION_ANALYSIS.md](SMUL_INSTRUCTION_ANALYSIS.md) |

## Logical and Bitwise Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| AND | ExAND.cs | ✅ | [AND_INSTRUCTION_ANALYSIS.md](AND_INSTRUCTION_ANALYSIS.md) |
| OR | ExOR.cs | ✅ | [OR_INSTRUCTION_ANALYSIS.md](OR_INSTRUCTION_ANALYSIS.md) |
| XOR | ExXOR.cs | ✅ | [XOR_INSTRUCTION_ANALYSIS.md](XOR_INSTRUCTION_ANALYSIS.md) |
| NAND | ExNAND.cs | ✅ | [NAND_INSTRUCTION_ANALYSIS.md](NAND_INSTRUCTION_ANALYSIS.md) |
| NOR | ExNOR.cs | ✅ | [NOR_INSTRUCTION_ANALYSIS.md](NOR_INSTRUCTION_ANALYSIS.md) |
| XNOR | ExXNOR.cs | ✅ | [XNOR_INSTRUCTION_ANALYSIS.md](XNOR_INSTRUCTION_ANALYSIS.md) |
| IMPLY | ExIMPLY.cs | ✅ | [IMPLY_INSTRUCTION_ANALYSIS.md](IMPLY_INSTRUCTION_ANALYSIS.md) |
| INV | ExINV.cs | ✅ | [INV_INSTRUCTION_ANALYSIS.md](INV_INSTRUCTION_ANALYSIS.md) |
| INVF | ExINVF.cs | ✅ | [INVF_INSTRUCTION_ANALYSIS.md](INVF_INSTRUCTION_ANALYSIS.md) |

## Bit Manipulation Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| SET | ExSET.cs | ✅ | [SET_INSTRUCTION_ANALYSIS.md](SET_INSTRUCTION_ANALYSIS.md) |
| RES | ExRES.cs | ✅ | [RES_INSTRUCTION_ANALYSIS.md](RES_INSTRUCTION_ANALYSIS.md) |
| SETF | ExSETF.cs | ✅ | [SETF_INSTRUCTION_ANALYSIS.md](SETF_INSTRUCTION_ANALYSIS.md) |
| RESF | ExRESF.cs | ✅ | [RESF_INSTRUCTION_ANALYSIS.md](RESF_INSTRUCTION_ANALYSIS.md) |
| BIT | ExBIT.cs | ✅ | [BIT_INSTRUCTION_ANALYSIS.md](BIT_INSTRUCTION_ANALYSIS.md) |
| SETBITS | ExSETBITS.cs | ✅ | [SETBITS_INSTRUCTION_ANALYSIS.md](SETBITS_INSTRUCTION_ANALYSIS.md) |
| GETBITS | ExGETBITS.cs | ✅ | [GETBITS_INSTRUCTION_ANALYSIS.md](GETBITS_INSTRUCTION_ANALYSIS.md) |

## Shift and Rotate Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| SL | ExSL.cs | ✅ | [SL_INSTRUCTION_ANALYSIS.md](SL_INSTRUCTION_ANALYSIS.md) |
| SR | ExSR.cs | ✅ | [SR_INSTRUCTION_ANALYSIS.md](SR_INSTRUCTION_ANALYSIS.md) |
| RL | ExRL.cs | ✅ | [RL_INSTRUCTION_ANALYSIS.md](RL_INSTRUCTION_ANALYSIS.md) |
| RR | ExRR.cs | ✅ | [RR_INSTRUCTION_ANALYSIS.md](RR_INSTRUCTION_ANALYSIS.md) |

## Comparison and Control Flow Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| CP | ExCP.cs | ✅ | [CP_INSTRUCTION_ANALYSIS.md](CP_INSTRUCTION_ANALYSIS.md) |
| SCP | ExSCP.cs | ✅ | [SCP_INSTRUCTION_ANALYSIS.md](SCP_INSTRUCTION_ANALYSIS.md) |
| CBR | ExCBR.cs | ✅ | [CBR_INSTRUCTION_ANALYSIS.md](CBR_INSTRUCTION_ANALYSIS.md) |
| JP | ExJP.cs | ✅ | [JP_INSTRUCTION_ANALYSIS.md](JP_INSTRUCTION_ANALYSIS.md) |
| JR | ExJR.cs | ✅ | [JR_INSTRUCTION_ANALYSIS.md](JR_INSTRUCTION_ANALYSIS.md) |
| CALL | ExCALL.cs | ✅ | [CALL_INSTRUCTION_ANALYSIS.md](CALL_INSTRUCTION_ANALYSIS.md) |
| CALLR | ExCALLR.cs | ✅ | [CALLR_INSTRUCTION_ANALYSIS.md](CALLR_INSTRUCTION_ANALYSIS.md) |
| RET | ExMisc.cs | ✅ | [RET_INSTRUCTION_ANALYSIS.md](RET_INSTRUCTION_ANALYSIS.md) |
| RETIF | ExRETIF.cs | ✅ | [RETIF_INSTRUCTION_ANALYSIS.md](RETIF_INSTRUCTION_ANALYSIS.md) |

## Increment and Decrement Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| INC | ExINC.cs | ✅ | [INC_INSTRUCTION_ANALYSIS.md](INC_INSTRUCTION_ANALYSIS.md) |
| DEC | ExDEC.cs | ✅ | [DEC_INSTRUCTION_ANALYSIS.md](DEC_INSTRUCTION_ANALYSIS.md) |
| DJNZ | ExDJNZ.cs | ✅ | [DJNZ_INSTRUCTION_ANALYSIS.md](DJNZ_INSTRUCTION_ANALYSIS.md) |

## Stack Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| PUSH | ExPUSH.cs | ✅ | [PUSH_INSTRUCTION_ANALYSIS.md](PUSH_INSTRUCTION_ANALYSIS.md) |
| POP | ExPOP.cs | ✅ | [POP_INSTRUCTION_ANALYSIS.md](POP_INSTRUCTION_ANALYSIS.md) |

## Exchange Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| EX | ExEX.cs | ✅ | [EX_INSTRUCTION_ANALYSIS.md](EX_INSTRUCTION_ANALYSIS.md) |

## Floating-Point Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| LDF | ExLDF.cs | ✅ | [LDF_INSTRUCTION_ANALYSIS.md](LDF_INSTRUCTION_ANALYSIS.md) |
| FREGS | ExFREGS.cs | ✅ | [FREGS_INSTRUCTION_ANALYSIS.md](FREGS_INSTRUCTION_ANALYSIS.md) |

## Mathematical Functions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| ABS | ExABS.cs | ✅ | [ABS_INSTRUCTION_ANALYSIS.md](ABS_INSTRUCTION_ANALYSIS.md) |
| SQR | ExSQR.cs | ✅ | [SQR_INSTRUCTION_ANALYSIS.md](SQR_INSTRUCTION_ANALYSIS.md) |
| ISQR | ExISQR.cs | ✅ | [ISQR_INSTRUCTION_ANALYSIS.md](ISQR_INSTRUCTION_ANALYSIS.md) |
| POW | ExPOW.cs | ✅ | [POW_INSTRUCTION_ANALYSIS.md](POW_INSTRUCTION_ANALYSIS.md) |
| SIN | ExSIN.cs | ✅ | [SIN_INSTRUCTION_ANALYSIS.md](SIN_INSTRUCTION_ANALYSIS.md) |
| COS | ExCOS.cs | ✅ | [COS_INSTRUCTION_ANALYSIS.md](COS_INSTRUCTION_ANALYSIS.md) |
| TAN | ExTAN.cs | ✅ | [TAN_INSTRUCTION_ANALYSIS.md](TAN_INSTRUCTION_ANALYSIS.md) |
| MIN | ExMIN.cs | ✅ | [MIN_INSTRUCTION_ANALYSIS.md](MIN_INSTRUCTION_ANALYSIS.md) |
| MAX | ExMAX.cs | ✅ | [MAX_INSTRUCTION_ANALYSIS.md](MAX_INSTRUCTION_ANALYSIS.md) |
| ROUND | ExROUND.cs | ✅ | [ROUND_INSTRUCTION_ANALYSIS.md](ROUND_INSTRUCTION_ANALYSIS.md) |
| FLOOR | ExFLOOR.cs | ✅ | [FLOOR_INSTRUCTION_ANALYSIS.md](FLOOR_INSTRUCTION_ANALYSIS.md) |
| CEIL | ExCEIL.cs | ✅ | [CEIL_INSTRUCTION_ANALYSIS.md](CEIL_INSTRUCTION_ANALYSIS.md) |
| INT | ExINT.cs | ✅ | [INT_INSTRUCTION_ANALYSIS.md](INT_INSTRUCTION_ANALYSIS.md) |
| ISGN | ExISGN.cs | ✅ | [ISGN_INSTRUCTION_ANALYSIS.md](ISGN_INSTRUCTION_ANALYSIS.md) |

## Color Space Conversion Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| RGB2HSL | ExC_RGB2HSL.cs | ✅ | [RGB2HSL_INSTRUCTION_ANALYSIS.md](RGB2HSL_INSTRUCTION_ANALYSIS.md) |
| HSL2RGB | ExC_HSL2RGB.cs | ✅ | [HSL2RGB_INSTRUCTION_ANALYSIS.md](HSL2RGB_INSTRUCTION_ANALYSIS.md) |
| RGB2HSB | ExC_RGB2HSB.cs | ✅ | [RGB2HSB_INSTRUCTION_ANALYSIS.md](RGB2HSB_INSTRUCTION_ANALYSIS.md) |
| HSB2RGB | ExC_HSB2RGB.cs | ✅ | [HSB2RGB_INSTRUCTION_ANALYSIS.md](HSB2RGB_INSTRUCTION_ANALYSIS.md) |

## Memory Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| MEMC | ExMEMC.cs | ✅ | [MEMC_INSTRUCTION_ANALYSIS.md](MEMC_INSTRUCTION_ANALYSIS.md) |
| MEMF | ExMEMF.cs | ✅ | [MEMF_INSTRUCTION_ANALYSIS.md](MEMF_INSTRUCTION_ANALYSIS.md) |
| FIND | ExFIND.cs | ✅ | [FIND_INSTRUCTION_ANALYSIS.md](FIND_INSTRUCTION_ANALYSIS.md) |

## Register Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| REGS | ExREGS.cs | ✅ | [REGS_INSTRUCTION_ANALYSIS.md](REGS_INSTRUCTION_ANALYSIS.md) |
| LDREGS | ExLDREGS.cs | ✅ | [LDREGS_INSTRUCTION_ANALYSIS.md](LDREGS_INSTRUCTION_ANALYSIS.md) |
| STREGS | ExSTREGS.cs | ✅ | [STREGS_INSTRUCTION_ANALYSIS.md](STREGS_INSTRUCTION_ANALYSIS.md) |

## Variable Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| GETVAR | ExGETVAR.cs | ✅ | [GETVAR_INSTRUCTION_ANALYSIS.md](GETVAR_INSTRUCTION_ANALYSIS.md) |
| SETVAR | ExSETVAR.cs | ✅ | [SETVAR_INSTRUCTION_ANALYSIS.md](SETVAR_INSTRUCTION_ANALYSIS.md) |

## Graphics Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| VCL | ExVCL.cs | ✅ | [VCL_INSTRUCTION_ANALYSIS.md](VCL_INSTRUCTION_ANALYSIS.md) |
| VDL | ExVDL.cs | ✅ | [VDL_INSTRUCTION_ANALYSIS.md](VDL_INSTRUCTION_ANALYSIS.md) |

## Audio Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| PLAY | ExPLAY.cs | ✅ | [PLAY_INSTRUCTION_ANALYSIS.md](PLAY_INSTRUCTION_ANALYSIS.md) |

## System Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| INT | ExINT.cs | ✅ | [INT_INSTRUCTION_ANALYSIS.md](INT_INSTRUCTION_ANALYSIS.md) |
| WAIT | ExWAIT.cs | ✅ | [WAIT_INSTRUCTION_ANALYSIS.md](WAIT_INSTRUCTION_ANALYSIS.md) |
| RAND | ExRAND.cs | ✅ | [RAND_INSTRUCTION_ANALYSIS.md](RAND_INSTRUCTION_ANALYSIS.md) |
| DEBUG | ExDEBUG.cs | ✅ | [DEBUG_INSTRUCTION_ANALYSIS.md](DEBUG_INSTRUCTION_ANALYSIS.md) |

## Miscellaneous Instructions

| Instruction | File | Status | Documentation |
|------------|------|--------|--------------|
| BREAK | ExMisc.cs | ✅ | [BREAK_INSTRUCTION_ANALYSIS.md](BREAK_INSTRUCTION_ANALYSIS.md) |

## Documentation Template

When creating documentation for a new instruction, use the following template:

```markdown
# [INSTRUCTION] Instruction Execution Analysis

## Executive Summary

Brief overview of the instruction, its purpose, and key characteristics.

## File Statistics

- **File**: `Emulator/Execution/Ex[INSTRUCTION].cs`
- **Total Lines**: [number]
- **Instruction Variants**: [number] unique opcodes
- **Implementation Pattern**: [dispatch table/switch statement/etc.]

## Addressing Modes

Detailed description of all addressing modes supported.

## Operation Categories

Breakdown of different operation types.

## Flag Updates

Description of which flags are updated and when.

## Implementation Details

Technical details about the implementation.

## Usage Examples

Example code snippets.

## Conclusion

Summary and notes.
```

## Notes

- All instruction files are located in `Continuum Emulator/Emulator/Execution/`
- Instruction mnemonics are defined in `Continuum Emulator/Emulator/Mnemonics/Mnemonic.cs`
- The LD instruction has the most comprehensive documentation as it's the most complex
- Many instructions share similar patterns (e.g., ADD/SUB, AND/OR/XOR)

## Next Steps

1. Document core control flow instructions (JP, JR, CALL, CALLR)
2. Document comparison instruction (CP)
3. Document bit manipulation instructions (SET, RES, BIT)
4. Document shift/rotate instructions (SL, SR, RL, RR)
5. Document remaining instructions systematically

