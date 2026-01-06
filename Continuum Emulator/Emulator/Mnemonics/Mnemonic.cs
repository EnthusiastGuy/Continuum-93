using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.Mnemonics
{
    public class Mnem
    {
        #region register pointers
        public const byte A = 0x00, AB = 0x00, ABC = 0x00, ABCD = 0x00;
        public const byte B = 0x01, BC = 0x01, BCD = 0x01, BCDE = 0x01;
        public const byte C = 0x02, CD = 0x02, CDE = 0x02, CDEF = 0x02;
        public const byte D = 0x03, DE = 0x03, DEF = 0x03, DEFG = 0x03;
        public const byte E = 0x04, EF = 0x04, EFG = 0x04, EFGH = 0x04;
        public const byte F = 0x05, FG = 0x05, FGH = 0x05, FGHI = 0x05;
        public const byte G = 0x06, GH = 0x06, GHI = 0x06, GHIJ = 0x06;
        public const byte H = 0x07, HI = 0x07, HIJ = 0x07, HIJK = 0x07;
        public const byte I = 0x08, IJ = 0x08, IJK = 0x08, IJKL = 0x08;
        public const byte J = 0x09, JK = 0x09, JKL = 0x09, JKLM = 0x09;
        public const byte K = 0x0A, KL = 0x0A, KLM = 0x0A, KLMN = 0x0A;
        public const byte L = 0x0B, LM = 0x0B, LMN = 0x0B, LMNO = 0x0B;
        public const byte M = 0x0C, MN = 0x0C, MNO = 0x0C, MNOP = 0x0C;
        public const byte N = 0x0D, NO = 0x0D, NOP = 0x0D, NOPQ = 0x0D;
        public const byte O = 0x0E, OP = 0x0E, OPQ = 0x0E, OPQR = 0x0E;
        public const byte P = 0x0F, PQ = 0x0F, PQR = 0x0F, PQRS = 0x0F;
        public const byte Q = 0x10, QR = 0x10, QRS = 0x10, QRST = 0x10;
        public const byte R = 0x11, RS = 0x11, RST = 0x11, RSTU = 0x11;
        public const byte S = 0x12, ST = 0x12, STU = 0x12, STUV = 0x12;
        public const byte T = 0x13, TU = 0x13, TUV = 0x13, TUVW = 0x13;
        public const byte U = 0x14, UV = 0x14, UVW = 0x14, UVWX = 0x14;
        public const byte V = 0x15, VW = 0x15, VWX = 0x15, VWXY = 0x15;
        public const byte W = 0x16, WX = 0x16, WXY = 0x16, WXYZ = 0x16;
        public const byte X = 0x17, XY = 0x17, XYZ = 0x17, XYZA = 0x17;
        public const byte Y = 0x18, YZ = 0x18, YZA = 0x18, YZAB = 0x18;
        public const byte Z = 0x19, ZA = 0x19, ZAB = 0x19, ZABC = 0x19;

        #endregion

        #region main op codes
        public const byte NOOP = 0;
        public const byte LD = 1;
        public const byte ADD = 2;
        public const byte SUB = 3;
        public const byte DIV = 4;
        public const byte MUL = 5;

        public const byte SL = 6;
        public const byte SR = 7;
        public const byte RL = 8;
        public const byte RR = 9;

        public const byte SET = 10;
        public const byte RES = 11;

        public const byte BIT = 12;

        public const byte AND = 13, AND16 = AND, AND24 = AND, AND32 = AND;
        public const byte OR = 14, OR16 = OR, OR24 = OR, OR32 = OR;
        public const byte XOR = 15, XOR16 = XOR, XOR24 = XOR, XOR32 = XOR;

        public const byte INV = 16;

        public const byte EX = 17;

        public const byte CP = 18;

        public const byte INC = 19, INC16 = INC, INC24 = INC, INC32 = INC;
        public const byte DEC = 20, DEC16 = DEC, DEC24 = DEC, DEC32 = DEC;


        // CALL, JUMP
        public const byte CALL = 21;
        public const byte CALLR = 22;
        public const byte JP = 23;
        public const byte JR = 24;

        // MOD (remainder) - mirrors DIV addressing matrix
        public const byte MOD = 25;

        public const byte POP = 26, POP16 = POP, POP24 = POP, POP32 = POP;
        public const byte PUSH = 28, PUSH16 = PUSH, PUSH24 = PUSH, PUSH32 = PUSH;

        public const byte INT = 29;

        public const byte RAND = 30;

        public const byte MIN = 31;
        public const byte MAX = 32;

        public const byte DJNZ = 33, DJNZ16 = DJNZ, DJNZ24 = DJNZ, DJNZ32 = DJNZ;

        public const byte SETF = 36;
        public const byte RESF = 37;
        public const byte INVF = 38;
        public const byte LDF = 39;

        public const byte REGS = 40;
        public const byte WAIT = 41;

        public const byte VDL = 42;
        public const byte VCL = 43;

        public const byte FIND = 44;

        // 43-47 available

        public const byte IMPLY = 48;
        public const byte NAND = 49;
        public const byte NOR = 50;
        public const byte XNOR = 51;

        // 52-55 available
        public const byte GETBITS = 56;
        public const byte SETBITS = 57;

        // 58-63 available

        public const byte MEMF = 64;
        public const byte MEMC = 65;

        // 66-69 available

        public const byte LDREGS = 70;
        public const byte STREGS = 71;

        // 72-91 available

        // Signed division, multiplication and compare
        public const byte SDIV = 92;
        public const byte SMUL = 93;
        public const byte SCP = 94;

        public const byte FREGS = 95;

        // Float point math
        public const byte POW = 96;
        public const byte SQR = 97;
        public const byte CBR = 98;
        public const byte ISQR = 99;   // Inverse square
        public const byte ISGN = 100;   // Invert sign of float

        // 101 available

        public const byte SIN = 102;
        public const byte COS = 103;
        public const byte TAN = 104;

        // 105-127 available

        public const byte ABS = 128;
        public const byte ROUND = 129;
        public const byte FLOOR = 130;
        public const byte CEIL = 131;

        // 132-159 available

        public const byte PLAY = 160;

        // 161-189 available

        public const byte SETVAR = 190;
        public const byte GETVAR = 191;

        public const byte RGB2HSL = 192;
        public const byte HSL2RGB = 193;

        public const byte RGB2HSB = 194;
        public const byte HSB2RGB = 195;

        // 204-252 available

        // End instructions
        public const byte DEBUG = 252;

        public const byte BREAK = 253;
        public const byte RETIF = 254;
        public const byte RET = 255;

        #endregion

        #region secondary op codes
        


        // DIV
        public const byte DIVMUL_r_n = 0;
        public const byte DIVMUL_r_r = 1;

        public const byte DIVMUL_rr_n = 2;
        public const byte DIVMUL_rr_r = 3;
        public const byte DIVMUL_rr_rr = 4;

        public const byte DIVMUL_rrr_n = 5;
        public const byte DIVMUL_rrr_r = 6;
        public const byte DIVMUL_rrr_rr = 7;
        public const byte DIVMUL_rrr_rrr = 8;

        public const byte DIVMUL_rrrr_n = 9;
        public const byte DIVMUL_rrrr_r = 10;
        public const byte DIVMUL_rrrr_rr = 11;
        public const byte DIVMUL_rrrr_rrr = 12;
        public const byte DIVMUL_rrrr_rrrr = 13;

        public const byte DIV_r_n_r = 14;
        public const byte DIV_r_r_r = 15;

        public const byte DIV_rr_n_rr = 16;
        public const byte DIV_rr_r_r = 17;
        public const byte DIV_rr_rr_rr = 18;

        public const byte DIV_rrr_n_rr = 19;
        public const byte DIV_rrr_r_r = 20;
        public const byte DIV_rrr_rr_rr = 21;
        public const byte DIV_rrr_rrr_rrr = 22;

        public const byte DIV_rrrr_n_rr = 23;
        public const byte DIV_rrrr_r_r = 24;
        public const byte DIV_rrrr_rr_rr = 25;
        public const byte DIV_rrrr_rrr_rrr = 26;
        public const byte DIV_rrrr_rrrr_rrrr = 27;

        // Float
        public const byte DIVMUL_fr_fr = 28;
        public const byte DIVMUL_fr_nnn = 29;
        public const byte DIVMUL_fr_r = 30;
        public const byte DIVMUL_fr_rr = 31;
        public const byte DIVMUL_fr_rrr = 32;
        public const byte DIVMUL_fr_rrrr = 33;

        public const byte DIVMUL_r_fr = 34;
        public const byte DIVMUL_rr_fr = 35;
        public const byte DIVMUL_rrr_fr = 36;
        public const byte DIVMUL_rrrr_fr = 37;

        public const byte DIVMUL_fr_InnnI = 38;
        public const byte DIVMUL_fr_IrrrI = 39;
        public const byte DIVMUL_InnnI_fr = 40;
        public const byte DIVMUL_IrrrI_fr = 41;

        // SHIFT, ROLL, SET, RES
        public const byte SHRLSTRE_r_n = 0b_000000;
        public const byte SHRLSTRE_r_r = 0b_000001;

        public const byte SHRLSTRE_rr_n = 0b_000010;
        public const byte SHRLSTRE_rr_r = 0b_000011;

        public const byte SHRLSTRE_rrr_n = 0b_000100;
        public const byte SHRLSTRE_rrr_r = 0b_000101;

        public const byte SHRLSTRE_rrrr_n = 0b_000110;
        public const byte SHRLSTRE_rrrr_r = 0b_000111;

        // AND, OR, XOR, NAND, NOR, XNOR, IMPLY
        public const byte AOX_r_n = 0;
        public const byte AOX_r_r = 1;

        public const byte AOX_rr_nn = 2;
        public const byte AOX_rr_rr = 3;

        public const byte AOX_rrr_nnn = 4;
        public const byte AOX_rrr_rrr = 5;

        public const byte AOX_rrrr_nnnn = 6;
        public const byte AOX_rrrr_rrrr = 7;

        public const byte AOX_IrrrI_n = 8;
        public const byte AOX16_IrrrI_nn = 9;
        public const byte AOX24_IrrrI_nnn = 10;
        public const byte AOX32_IrrrI_nnnn = 11;

        public const byte AOX_IrrrI_r = 12;
        public const byte AOX_IrrrI_rr = 13;
        public const byte AOX_IrrrI_rrr = 14;
        public const byte AOX_IrrrI_rrrr = 15;

        // new
        public const byte AOX_r_IrrrI = 16;
        public const byte AOX_rr_IrrrI = 17;
        public const byte AOX_rrr_IrrrI = 18;
        public const byte AOX_rrrr_IrrrI = 19;

        public const byte AOX_InnnI_n = 20;
        public const byte AOX16_InnnI_nn = 21;
        public const byte AOX24_InnnI_nnn = 22;
        public const byte AOX32_InnnI_nnnn = 23;

        public const byte AOX_InnnI_r = 24;
        public const byte AOX_InnnI_rr = 25;
        public const byte AOX_InnnI_rrr = 26;
        public const byte AOX_InnnI_rrrr = 27;

        public const byte AOX_r_InnnI = 28;
        public const byte AOX_rr_InnnI = 29;
        public const byte AOX_rrr_InnnI = 30;
        public const byte AOX_rrrr_InnnI = 31;

        //INV
        public const byte INV_r = 0b_0000;
        public const byte INV_rr = 0b_0001;
        public const byte INV_rrr = 0b_0010;
        public const byte INV_rrrr = 0b_0011;

        // new
        public const byte INV_IrrrI = 0b_0100;
        public const byte INV16_IrrrI = 0b_0101;
        public const byte INV24_IrrrI = 0b_0110;
        public const byte INV32_IrrrI = 0b_0111;
        public const byte INV_InnnI = 0b_1000;
        public const byte INV16_InnnI = 0b_1001;
        public const byte INV24_InnnI = 0b_1010;
        public const byte INV32_InnnI = 0b_1011;

        // EX
        public const byte EX_r_r = 0b_00000;
        public const byte EX_rr_rr = 0b_00001;
        public const byte EX_rrr_rrr = 0b_00010;
        public const byte EX_rrrr_rrrr = 0b_00011;
        // Float
        public const byte EX_fr_fr = 0b_00100;

        // CP
        public const byte CP_r_n = 0;
        public const byte CP_r_r = 1;
        public const byte CP_rr_nn = 2;
        public const byte CP_rr_rr = 3;
        public const byte CP_rrr_nnn = 4;
        public const byte CP_rrr_rrr = 5;
        public const byte CP_rrrr_nnnn = 6;
        public const byte CP_rrrr_rrrr = 7;

        public const byte CP_r_IrrrI = 8;
        public const byte CP_rr_IrrrI = 9;
        public const byte CP_rrr_IrrrI = 10;
        public const byte CP_rrrr_IrrrI = 11;
        public const byte CP_IrrrI_IrrrI = 12;
        public const byte CP_IrrrI_nnn = 13;

        // Floating point register operations
        public const byte CP_fr_fr = 14;
        public const byte CP_fr_nnn = 15;
        public const byte CP_fr_r = 16;
        public const byte CP_fr_rr = 17;
        public const byte CP_fr_rrr = 18;
        public const byte CP_fr_rrrr = 19;
        public const byte CP_r_fr = 20;
        public const byte CP_rr_fr = 21;
        public const byte CP_rrr_fr = 22;
        public const byte CP_rrrr_fr = 23;
        public const byte CP_fr_InnnI = 24;
        public const byte CP_fr_IrrrI = 25;
        public const byte CP_InnnI_fr = 26;
        public const byte CP_IrrrI_fr = 27;

        // INC, DEC
        public const byte INCDEC_r = 0;
        public const byte INCDEC_rr = 1;
        public const byte INCDEC_rrr = 2;
        public const byte INCDEC_rrrr = 3;
        public const byte INCDEC_IrrrI = 4;
        public const byte INCDEC16_IrrrI = 5;
        public const byte INCDEC24_IrrrI = 6;
        public const byte INCDEC32_IrrrI = 7;
        public const byte INCDEC_InnnI = 8;
        public const byte INCDEC16_InnnI = 9;
        public const byte INCDEC24_InnnI = 10;
        public const byte INCDEC32_InnnI = 11;

        // CALL, CALLR, JP, JR
        public const byte CLRJ_nnn = 0b_00000;
        public const byte CLRJ_ff_nnn = 0b_00001;
        public const byte CLRJ_rrr = 0b_00010;
        public const byte CLRJ_ff_rrr = 0b_00011;

        // POP, PUSH
        public const byte POPU_r = 0b_0000;
        public const byte POPU_rr = 0b_0001;
        public const byte POPU_rrr = 0b_0010;
        public const byte POPU_rrrr = 0b_0011;
        public const byte POPU_r_r = 0b_0100;

        public const byte POPU_fr = 0b_0101;
        public const byte POPU_fr_fr = 0b_0110;

        public const byte POPU_InnnI = 0b_0111;
        public const byte POPU_IrrrI = 0b_1000;
        public const byte POPU16_InnnI = 0b_1001;
        public const byte POPU16_IrrrI = 0b_1010;
        public const byte POPU24_InnnI = 0b_1011;
        public const byte POPU24_IrrrI = 0b_1100;
        public const byte POPU32_InnnI = 0b_1101;
        public const byte POPU32_IrrrI = 0b_1110;

        // INT
        public const byte INT_n_r = 0b_000;

        // RAND
        public const byte RAND_r = 0;
        public const byte RAND_rr = 1;
        public const byte RAND_rrr = 2;
        public const byte RAND_rrrr = 3;

        public const byte RAND_r_n = 4;
        public const byte RAND_rr_nn = 5;
        public const byte RAND_rrr_nnn = 6;
        public const byte RAND_rrrr_nnnn = 7;

        public const byte RAND_fr = 8;
        public const byte RAND_fr_nnnn = 9;
        public const byte RAND_fr_rrrr = 10;

        // MIN/MAX
        public const byte MIAX_r_r = 0b_000000;
        public const byte MIAX_rr_rr = 0b_000001;
        public const byte MIAX_rrr_rrr = 0b_000010;
        public const byte MIAX_rrrr_rrrr = 0b_000011;

        public const byte MIAX_r_n = 0b_000100;
        public const byte MIAX_rr_nn = 0b_000101;
        public const byte MIAX_rrr_nnn = 0b_000110;
        public const byte MIAX_rrrr_nnnn = 0b_000111;

        public const byte MIAX_fr_n = 0b_001000;
        public const byte MIAX_fr_fr = 0b_001001;
        public const byte MIAX_r_fr = 0b_001010;
        public const byte MIAX_rr_fr = 0b_001011;
        public const byte MIAX_rrr_fr = 0b_001100;
        public const byte MIAX_rrrr_fr = 0b_001101;

        public const byte MIAX_fr_r = 0b_001110;
        public const byte MIAX_fr_rr = 0b_001111;
        public const byte MIAX_fr_rrr = 0b_010000;
        public const byte MIAX_fr_rrrr = 0b_010001;

        // DJNZ
        public const byte DJNZ_r_nnn = 0;
        public const byte DJNZ_r_rrr = 1;
        public const byte DJNZ_rr_nnn = 2;
        public const byte DJNZ_rr_rrr = 3;
        public const byte DJNZ_rrr_nnn = 4;
        public const byte DJNZ_rrr_rrr = 5;
        public const byte DJNZ_rrrr_nnn = 6;
        public const byte DJNZ_rrrr_rrr = 7;

        public const byte DJNZ_InnnI_nnn = 8;
        public const byte DJNZ_InnnI_rrr = 9;
        public const byte DJNZ16_InnnI_nnn = 10;
        public const byte DJNZ16_InnnI_rrr = 11;
        public const byte DJNZ24_InnnI_nnn = 12;
        public const byte DJNZ24_InnnI_rrr = 13;
        public const byte DJNZ32_InnnI_nnn = 14;
        public const byte DJNZ32_InnnI_rrr = 15;

        public const byte DJNZ_IrrrI_nnn = 16;
        public const byte DJNZ_IrrrI_rrr = 17;
        public const byte DJNZ16_IrrrI_nnn = 18;
        public const byte DJNZ16_IrrrI_rrr = 19;
        public const byte DJNZ24_IrrrI_nnn = 20;
        public const byte DJNZ24_IrrrI_rrr = 21;
        public const byte DJNZ32_IrrrI_nnn = 22;
        public const byte DJNZ32_IrrrI_rrr = 23;

        // STRSIVF
        public const byte STRSIVF_ff = 0;

        // LDF
        public const byte LDF_r = 0b_000;
        public const byte LDF_IrrrI = 0b_001;
        public const byte LDF_InnnI = 0b_010;
        public const byte LDF_r_n = 0b_011;
        public const byte LDF_IrrrI_n = 0b_100;
        public const byte LDF_InnnI_n = 0b_101;

        // REGS
        public const byte REGS_n = 0b_000;
        public const byte REGS_InnnI = 0b_001;
        public const byte REGS_r = 0b_010;
        public const byte REGS_IrrrI = 0b_011;

        // WAIT
        public const byte WAIT_n = 0b_0;

        // VDL/VCL
        public const byte VDCL_n = 0b_000000;
        public const byte VDCL_r = 0b_000001;
        public const byte VDCL_InnnI = 0b_000010;
        public const byte VDCL_IrrrI = 0b_000011;

        // FIND
        public const byte FIND_IrrrI_n = 0b_000;
        public const byte FIND_IrrrI_InnnI = 0b_001;
        public const byte FIND_IrrrI_IrrrI = 0b_010;

        // GETBITS
        public const byte GETBITS_r_rrrr_n = 0;
        public const byte GETBITS_rr_rrrr_n = 1;
        public const byte GETBITS_rrr_rrrr_n = 2;
        public const byte GETBITS_rrrr_rrrr_n = 3;
        public const byte GETBITS_r_rrrr_r = 4;
        public const byte GETBITS_rr_rrrr_r = 5;
        public const byte GETBITS_rrr_rrrr_r = 6;
        public const byte GETBITS_rrrr_rrrr_r = 7;

        // SETBITS
        public const byte SETBITS_rrrr_r_n = 0;
        public const byte SETBITS_rrrr_rr_n = 1;
        public const byte SETBITS_rrrr_rrr_n = 2;
        public const byte SETBITS_rrrr_rrrr_n = 3;
        public const byte SETBITS_rrrr_r_r = 4;
        public const byte SETBITS_rrrr_rr_r = 5;
        public const byte SETBITS_rrrr_rrr_r = 6;
        public const byte SETBITS_rrrr_rrrr_r = 7;

        // MEMF
        public const byte MEMF_rrr_rrr_r = 0b_000;
        public const byte MEMF_nnn_rrr_r = 0b_001;
        public const byte MEMF_rrr_nnn_r = 0b_010;
        public const byte MEMF_nnn_nnn_r = 0b_011;
        public const byte MEMF_rrr_rrr_n = 0b_100;
        public const byte MEMF_nnn_rrr_n = 0b_101;
        public const byte MEMF_rrr_nnn_n = 0b_110;
        public const byte MEMF_nnn_nnn_n = 0b_111;

        // MEMC
        public const byte MEMC_rrr_rrr_rrr = 0b_000;
        public const byte MEMC_nnn_rrr_rrr = 0b_001;
        public const byte MEMC_rrr_nnn_rrr = 0b_010;
        public const byte MEMC_nnn_nnn_rrr = 0b_011;
        public const byte MEMC_rrr_rrr_nnn = 0b_100;
        public const byte MEMC_nnn_rrr_nnn = 0b_101;
        public const byte MEMC_rrr_nnn_nnn = 0b_110;
        public const byte MEMC_nnn_nnn_nnn = 0b_111;

        // LDREGS, STREGS
        public const byte LDSTREGS_r_r_IrrrI = 0b_0;
        public const byte LDSTREGS_r_r_InnnI = 0b_1;

        // SREGS
        // REGS
        public const byte FREGS_n = 0b_000;
        public const byte FREGS_InnnI = 0b_001;
        public const byte FREGS_r = 0b_010;
        public const byte FREGS_IrrrI = 0b_011;

        // POW
        public const byte POW_fr_fr = 0b_000000;
        public const byte POW_fr_r = 0b_000001;
        public const byte POW_fr_rr = 0b_000010;
        public const byte POW_fr_rrr = 0b_000011;
        public const byte POW_fr_rrrr = 0b_000100;

        public const byte POW_r_fr = 0b_000101;
        public const byte POW_rr_fr = 0b_000110;
        public const byte POW_rrr_fr = 0b_000111;
        public const byte POW_rrrr_fr = 0b_001000;

        public const byte POW_fr_IrrrI = 0b_001001;
        public const byte POW_IrrrI_fr = 0b_001010;
        public const byte POW_fr_nnn = 0b_001011;
        public const byte POW_fr_InnnI = 0b_001100;
        public const byte POW_InnnI_fr = 0b_001101;

        // SQRCR - square root, cube root
        public const byte SQRCR_fr = 0b_000000;
        public const byte SQRCR_r = 0b_000001;
        public const byte SQRCR_rr = 0b_000010;
        public const byte SQRCR_rrr = 0b_000011;
        public const byte SQRCR_rrrr = 0b_000100;
        public const byte SQRCR_IrrrI = 0b_000101;
        public const byte SQRCR_InnnI = 0b_000110;

        public const byte SQRCR_fr_fr = 0b_000111;
        public const byte SQRCR_fr_r = 0b_001000;
        public const byte SQRCR_fr_rr = 0b_001001;
        public const byte SQRCR_fr_rrr = 0b_001010;
        public const byte SQRCR_fr_rrrr = 0b_001011;
        public const byte SQRCR_fr_IrrrI = 0b_001100;
        public const byte SQRCR_fr_InnnI = 0b_001101;
        public const byte SQRCR_fr_n = 0b_001110;

        public const byte SQRCR_r_fr = 0b_001111;
        public const byte SQRCR_rr_fr = 0b_010000;
        public const byte SQRCR_rrr_fr = 0b_010001;
        public const byte SQRCR_rrrr_fr = 0b_010010;
        public const byte SQRCR_IrrrI_fr = 0b_010011;
        public const byte SQRCR_InnnI_fr = 0b_010100;

        // SIN, COS, TAN, CTG
        public const byte SINCTC_fr = 0b_0000;
        public const byte SINCTC_fr_fr = 0b_0001;

        // PLAY
        public const byte PLAY_nnn = 0;
        public const byte PLAY_rrr = 1;

        // SEVAR, GETVAR
        public const byte SGVAR_n_n = 0;
        public const byte SGVAR_n_rrrr = 1;
        public const byte SGVAR_rrrr_n = 2;
        public const byte SGVAR_rrrr_rrrr = 3;

        // RGB2HSL, RGB2HSB
        public const byte RGB2HSLB_nnn_rrrr = 0;
        public const byte RGB2HSLB_rrr_rrrr = 1;
        public const byte RGB2HSLB_nnn_InnnI = 2;
        public const byte RGB2HSLB_rrr_InnnI = 3;
        public const byte RGB2HSLB_nnn_IrrrI = 4;
        public const byte RGB2HSLB_rrr_IrrrI = 5;

        public const byte RGB2HSLB_InnnI_rrrr = 6;
        public const byte RGB2HSLB_IrrrI_rrrr = 7;
        public const byte RGB2HSLB_InnnI_InnnI = 8;
        public const byte RGB2HSLB_IrrrI_InnnI = 9;
        public const byte RGB2HSLB_InnnI_IrrrI = 10;
        public const byte RGB2HSLB_IrrrI_IrrrI = 11;

        // HSL2RGB, HSB2RGB
        public const byte HSLB2RGB_nnnn_rrr = 0;
        public const byte HSLB2RGB_rrrr_rrr = 1;
        public const byte HSLB2RGB_nnnn_InnnI = 2;
        public const byte HSLB2RGB_rrrr_InnnI = 3;
        public const byte HSLB2RGB_nnnn_IrrrI = 4;
        public const byte HSLB2RGB_rrrr_IrrrI = 5;

        public const byte HSLB2RGB_InnnI_rrr = 6;
        public const byte HSLB2RGB_IrrrI_rrr = 7;
        public const byte HSLB2RGB_InnnI_InnnI = 8;
        public const byte HSLB2RGB_IrrrI_InnnI = 9;
        public const byte HSLB2RGB_InnnI_IrrrI = 10;
        public const byte HSLB2RGB_IrrrI_IrrrI = 11;


        // RETIF
        public const byte RETIF_ff = 0b_0;
        public const byte RETIF_ff_ff = 0b_1;


        #endregion

        public static Dictionary<string, Oper> OPS = [];
        public static Dictionary<string, byte> REG = [];
        public static List<string> SREG = ["SPC", "SPR", "IPO"];      // Special registers

        static Mnem()
        {
            MainOperatorsInit.InitializeOperators();
            InitializeSubOperators();
            InitializeRegisters();
        }

        public static byte GetPrimaryOpCode(string opMnemonic)
        {
            // Maybe not mask failure with NOP
            return OPS.TryGetValue(opMnemonic, out Oper op) ? op.OpCode : NOOP;
        }

        public static bool PrimaryOpExists(string opMnemonic)
        {
            return OPS.TryGetValue(opMnemonic, out _);
        }

        public static byte? GetSecondaryOpCode(string opMnemonic)
        {
            byte? response = null;
            return OPS.TryGetValue(opMnemonic, out Oper op) ? op.OpCodes[0] : response;
        }

        public static byte? TryGetRegisterIndex(string registerName)
        {
            byte? response = null;
            return REG.TryGetValue(registerName, out byte op) ? op : response;
        }

        public static byte? TryGetFloatRegisterIndex(string registerName)
        {
            if (string.IsNullOrWhiteSpace(registerName))
                return null;

            string upper = registerName.ToUpper();
            string indexStr;

            if (upper.StartsWith("FR", StringComparison.Ordinal))
            {
                if (upper.Length <= 2)
                    return null;
                indexStr = upper[2..];
            }
            else if (upper.StartsWith("F", StringComparison.Ordinal))
            {
                if (upper.Length <= 1)
                    return null;
                indexStr = upper[1..];
            }
            else
            {
                return null;
            }

            if (byte.TryParse(indexStr, out byte index) && indexStr.All(char.IsDigit))
            {
                return index <= 15 ? index : null;
            }

            return null;
        }

        public static string GetOpFormat(string opMnemonic)
        {
            return OPS.TryGetValue(opMnemonic, out Oper op) ? op.Format : "";
        }

        public static bool IsSpecialRegister(string opMnemonic)
        {
            return SREG.Contains(opMnemonic);
        }

        public static Oper GetPrimaryOperator(byte primaryCode)
        {
            foreach (var op in OPS.Values)
            {
                if (op.OpCode == primaryCode)
                    return op;
            }
            return null;
        }

        public static bool IsSingleByteInstruction(byte primaryCode)
        {
            foreach (var op in OPS.Values)
            {
                if (op.ParentCode == primaryCode)
                    return false;
            }

            return true;
        }

        public static Oper GetSecondaryOperator(byte primaryCode, byte secondaryCode)
        {
            foreach (var op in OPS.Values)
            {
                if (op.ParentCode == primaryCode && op.MatchSubop(secondaryCode))
                    return op;
            }

            return null;
        }

        public static void AddPrimeOp(string mnemonic, byte code, string title = "", string[] description = null)
        {
            description ??= [];

            Oper oper = new(true, code)
            {
                Mnemonic = mnemonic,
                Title = title,
                Description = description
            };
            OPS.Add(mnemonic, oper);
        }

        public static void AddSubOp(string mnemonic, byte parentCode, string format, byte subCode)
        {
            OPS.Add(mnemonic, new Oper(false, parentCode, format, subCode)
            {
                Mnemonic = mnemonic,
            });
        }

        public static void InitializeSubOperators()
        {
            GenericInitializer.Initialize();    // Prototype


            OperatorsInitSHRL.Initialize();

            OperatorsInitSDIV.Initialize();
            OperatorsInitSMUL.Initialize();
            OperatorsInitRETIF.Initialize();
            
            OperatorsInitSETRES.Initialize();
            OperatorsInitBIT.Initialize();

            OperatorsInitAND.Initialize();
            OperatorsInitOR.Initialize();
            OperatorsInitXOR.Initialize();

            OperatorsInitNAND.Initialize();
            OperatorsInitNOR.Initialize();
            OperatorsInitXNOR.Initialize();

            OperatorsInitIMPLY.Initialize();

            OperatorsInitINV.Initialize();
            OperatorsInitEX.Initialize();
            OperatorsInitCP.Initialize();
            OperatorsInitSCP.Initialize();

            OperatorsInitINC.Initialize();
            OperatorsInitDEC.Initialize();

            OperatorsInitPLAY.Initialize();
            OperatorsInitSETVAR.Initialize();
            OperatorsInitGETVAR.Initialize();

            OperatorsInit_C_RGB2HSL.Initialize();
            OperatorsInit_C_HSL2RGB.Initialize();
            OperatorsInit_C_RGB2HSB.Initialize();
            OperatorsInit_C_HSB2RGB.Initialize();

            OperatorsInitGETBITS.Initialize();
            OperatorsInitSETBITS.Initialize();

            OperatorsInitDJNZ.Initialize();
            OperatorsInitFREGS.Initialize();
            OperatorsInitCALL_JUMP.Initialize();
            OperatorsInitPOP.Initialize();
            OperatorsInitPUSH.Initialize();
            OperatorsInitRAND.Initialize(); 
            OperatorsInitMIN_MAX.Initialize();

            OperatorsInitFlag_SET_RES_INV.Initialize();
            OperatorsInitLDF.Initialize();

            OperatorsInitREGS.Initialize();
            OperatorsInitVDL_VCL.Initialize();
            OperatorsInitFIND.Initialize();

            OperatorsInitMEMF_MEMC.Initialize();

            OperatorsInitLDREGS_STREGS.Initialize();

            OperatorsInitPOW.Initialize();
            OperatorsInitSQR.Initialize();
            OperatorsInitCBR.Initialize();
            OperatorsInitISQR.Initialize();

            OperatorsInitISGN.Initialize();

            OperatorsInit_TRIGONOMETRY.Initialize();
            OperatorsInit_MATH.Initialize();

            AddSubOp("INT nnn,r", INT, "oooBBBBB AAAAAAAA", INT_n_r);
            AddSubOp("WAIT nnn", WAIT, "oouuuuuu AAAAAAAA AAAAAAAA", WAIT_n);
        }

        public static void InitializeRegisters()
        {
            REG.Add("A", A);
            REG.Add("B", B);
            REG.Add("C", C);
            REG.Add("D", D);
            REG.Add("E", E);
            REG.Add("F", F);
            REG.Add("G", G);
            REG.Add("H", H);
            REG.Add("I", I);
            REG.Add("J", J);
            REG.Add("K", K);
            REG.Add("L", L);
            REG.Add("M", M);
            REG.Add("N", N);
            REG.Add("O", O);
            REG.Add("P", P);
            REG.Add("Q", Q);
            REG.Add("R", R);
            REG.Add("S", S);
            REG.Add("T", T);
            REG.Add("U", U);
            REG.Add("V", V);
            REG.Add("W", W);
            REG.Add("X", X);
            REG.Add("Y", Y);
            REG.Add("Z", Z);

            REG.Add("AB", AB);
            REG.Add("BC", BC);
            REG.Add("CD", CD);
            REG.Add("DE", DE);
            REG.Add("EF", EF);
            REG.Add("FG", FG);
            REG.Add("GH", GH);
            REG.Add("HI", HI);
            REG.Add("IJ", IJ);
            REG.Add("JK", JK);
            REG.Add("KL", KL);
            REG.Add("LM", LM);
            REG.Add("MN", MN);
            REG.Add("NO", NO);
            REG.Add("OP", OP);
            REG.Add("PQ", PQ);
            REG.Add("QR", QR);
            REG.Add("RS", RS);
            REG.Add("ST", ST);
            REG.Add("TU", TU);
            REG.Add("UV", UV);
            REG.Add("VW", VW);
            REG.Add("WX", WX);
            REG.Add("XY", XY);
            REG.Add("YZ", YZ);
            REG.Add("ZA", ZA);

            REG.Add("ABC", ABC);
            REG.Add("BCD", BCD);
            REG.Add("CDE", CDE);
            REG.Add("DEF", DEF);
            REG.Add("EFG", EFG);
            REG.Add("FGH", FGH);
            REG.Add("GHI", GHI);
            REG.Add("HIJ", HIJ);
            REG.Add("IJK", IJK);
            REG.Add("JKL", JKL);
            REG.Add("KLM", KLM);
            REG.Add("LMN", LMN);
            REG.Add("MNO", MNO);
            REG.Add("NOP", NOP);
            REG.Add("OPQ", OPQ);
            REG.Add("PQR", PQR);
            REG.Add("QRS", QRS);
            REG.Add("RST", RST);
            REG.Add("STU", STU);
            REG.Add("TUV", TUV);
            REG.Add("UVW", UVW);
            REG.Add("VWX", VWX);
            REG.Add("WXY", WXY);
            REG.Add("XYZ", XYZ);
            REG.Add("YZA", YZA);
            REG.Add("ZAB", ZAB);

            REG.Add("ABCD", ABCD);
            REG.Add("BCDE", BCDE);
            REG.Add("CDEF", CDEF);
            REG.Add("DEFG", DEFG);
            REG.Add("EFGH", EFGH);
            REG.Add("FGHI", FGHI);
            REG.Add("GHIJ", GHIJ);
            REG.Add("HIJK", HIJK);
            REG.Add("IJKL", IJKL);
            REG.Add("JKLM", JKLM);
            REG.Add("KLMN", KLMN);
            REG.Add("LMNO", LMNO);
            REG.Add("MNOP", MNOP);
            REG.Add("NOPQ", NOPQ);
            REG.Add("OPQR", OPQR);
            REG.Add("PQRS", PQRS);
            REG.Add("QRST", QRST);
            REG.Add("RSTU", RSTU);
            REG.Add("STUV", STUV);
            REG.Add("TUVW", TUVW);
            REG.Add("UVWX", UVWX);
            REG.Add("VWXY", VWXY);
            REG.Add("WXYZ", WXYZ);
            REG.Add("XYZA", XYZA);
            REG.Add("YZAB", YZAB);
            REG.Add("ZABC", ZABC);
        }
    }

    public class Oper
    {
        public string Mnemonic;
        public bool IsPrimary;
        public byte? ParentCode;
        public byte OpCode;
        public byte[] OpCodes;
        public string Format;
        public string Title;
        public string[] Description;

        private byte subOpBits;
        private byte opShift;

        public Oper(bool isPrimary, params byte[] codes) : this(isPrimary, null, "", codes)
        {
        }

        public Oper(bool isPrimary, byte? parentCode, string format, params byte[] codes)
        {
            IsPrimary = isPrimary;
            ParentCode = parentCode;
            Format = format;

            if (isPrimary)
            {
                OpCode = codes[0];
            }
            else
            {
                OpCodes = codes;
                SetAdvancedInfo();
            }
        }

        public void SetAdvancedInfo()
        {
            if (Format == null || Format.Length < 8)
                return;

            string opFormat = Format[..8];
            subOpBits = 0;
            foreach (char c in opFormat)
                if (c == 'o' || c == '0' || c == '1')
                    subOpBits++;

            opShift = (byte)(8 - subOpBits);
            string opBin = Convert.ToString(OpCodes[0] << opShift, 2).PadLeft(8, '0');
            string resultOp = "";
            for (byte i = 0; i < 8; i++)
            {
                resultOp += opBin[i] == '1' ? '1' : opFormat[i];
            }
        }

        public bool MatchSubop(byte subOp)
        {
            if (IsPrimary)
                return false;

            // TODO simplify this once the operands no longer use hardcoded bits next to the subOp
            string opFormat = Format[..8];
            byte subOpMask = 0;
            byte subOpBits = 0;
            for (byte i = 0; i < 8; i++)
            {
                if (opFormat[i] == '0')
                {
                    subOpBits++;
                }
                else if (opFormat[i] == '1')
                {
                    subOpMask |= (byte)(1 << (7 - i));
                    subOpBits++;
                }
            }

            byte CorrectedOpcode = OpCodes[0];

            if (subOpMask > 0)
            {
                CorrectedOpcode = (byte)((OpCodes[0] << subOpBits) | subOpMask);
            }


            return subOp >> opShift == CorrectedOpcode;     // OpCodes[0]
        }
    }
}
