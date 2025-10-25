using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_ADD
    {
        [Fact]
        public void TestFLAG_ZERO_ADD()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.MEMC.Set8bitToRAM(10000, 0x1);
            computer.MEMC.Set8bitToRAM(10005, 0x80);
            computer.MEMC.Set16bitToRAM(10010, 0x1);
            computer.MEMC.Set16bitToRAM(10015, 0x8000);
            computer.MEMC.Set24bitToRAM(10020, 0x1);
            computer.MEMC.Set24bitToRAM(10025, 0x800000);
            computer.MEMC.Set32bitToRAM(10030, 0x1);
            computer.MEMC.Set32bitToRAM(10035, 0x80000000);

            /*
                This is a string of direct assembly tests.
             */
            cp.Build(@"
                    LD A,0xFF   ; Register A gets his max value
                    ADD A, 0x1  ; We add 1
                    JP Z, .Ok1   ; The zero flag should now be set
                    LD X, 1     ; If not, error
                    BREAK       ; ... and exit
                .Ok1
                    LD A, 0xFF
                    LD B, 0x1
                    ADD A, B
                    JP Z, .Ok2
                    LD X, 2
                    BREAK
                .Ok2
                    LD A, 0xFF
                    ADD A, (10000)
                    JP Z, .Ok3
                    LD X, 3
                    BREAK
                .Ok3
                    LD A, 0xFF
                    LD BCD, 10000
                    ADD A, (BCD)
                    JP Z, .Ok4
                    LD X, 4
                    BREAK
                .Ok4
                    LD AB, 0xFFFF
                    ADD AB, 1
                    JP Z, .Ok5
                    LD X, 5
                    BREAK
                .Ok5
                    LD AB, 0x1
                    ADD16 AB, 0xFFFF
                    JP Z, .Ok6
                    LD X, 6
                    BREAK
                .Ok6
                    LD AB, 0x8000
                    LD CD, 0x8000
                    ADD AB, CD
                    JP Z, .Ok7
                    LD X, 7
                    BREAK
                .Ok7
                    LD AB, 0xFFFF
                    ADD AB, (10000)
                    JP Z, .Ok8
                    LD X, 8
                    BREAK
                .Ok8
                    LD AB, 0x8000
                    ADD16 AB, (10015)
                    JP Z, .Ok9
                    LD X, 9
                    BREAK
                .Ok9
                    LD AB, 0xFFFF
                    LD CDE, 10000
                    ADD AB, (CDE)
                    JP Z, .Ok10
                    LD X, 10
                    BREAK
                .Ok10
                    LD AB, 0x8000
                    LD CDE, 10015
                    ADD16 AB, (CDE)
                    JP Z, .Ok11
                    LD X, 11
                    BREAK
                .Ok11
                    LD ABC, 0xFFFFFF
                    ADD ABC, 1
                    JP Z, .Ok12
                    LD X, 12
                    BREAK
                .Ok12
                    LD ABC, 0xFF8000
                    ADD16 ABC, 0x8000
                    JP Z, .Ok13
                    LD X, 13
                    BREAK
                .Ok13
                    LD ABC, 0x800000
                    ADD24 ABC, 0x800000     ; ADDSUB__rrr_n
                    JP Z, .Ok14
                    LD X, 14
                    BREAK
                .Ok14
                    LD ABC, 0xFFFFFF
                    LD D, 1
                    ADD ABC, D              ; ADDSUB_rrr_r
                    JP Z, .Ok15
                    LD X, 15
                    BREAK
                .Ok15
                    LD ABC, 0xFF8000
                    LD DE, 0x8000
                    ADD ABC, DE             ; ADDSUB_rrr_rr
                    JP Z, .Ok16
                    LD X, 16
                    BREAK
                .Ok16
                    LD ABC, 0x800000
                    LD DEF, 0x800000
                    ADD ABC, DEF            ; ADDSUB_rrr_rrr
                    JP Z, .Ok17
                    LD X, 17
                    BREAK
                .Ok17
                    LD ABC, 0xFFFFFF
                    ADD ABC, (10000)        ; ADDSUB__rrr_InnnI - 8 bit
                    JP Z, .Ok18
                    LD X, 18
                    BREAK
                .Ok18
                    LD ABC, 0xFF8000
                    ADD16 ABC, (10015)      ; ADDSUB__rrr_InnnI - 16-bit
                    JP Z, .Ok19
                    LD X, 19
                    BREAK
                .Ok19
                    LD ABC, 0x800000
                    ADD24 ABC, (10025)      ; ADDSUB__rrr_InnnI - 24 bit
                    JP Z, .Ok20
                    LD X, 20
                    BREAK
                .Ok20
                    LD ABC, 0xFFFFFF
                    LD DEF, 10000
                    ADD ABC, (DEF)          ; ADDSUB_rrr_IrrrI
                    JP Z, .Ok21
                    LD X, 21
                    BREAK
                .Ok21
                    LD ABC, 0xFF8000
                    LD DEF, 10015
                    ADD16 ABC, (DEF)        ; ADDSUB16_rrr_IrrrI
                    JP Z, .Ok22
                    LD X, 22
                    BREAK
                .Ok22
                    LD ABC, 0x800000
                    LD DEF, 10025
                    ADD24 ABC, (DEF)        ; ADDSUB24_rrr_IrrrI
                    JP Z, .Ok23
                    LD X, 23
                    BREAK
                .Ok23
                    LD ABCD, 0xFFFFFFFF
                    ADD ABCD, 1             ; ADDSUB__rrrr_n - 8 bit
                    JP Z, .Ok24
                    LD X, 24
                    BREAK
                .Ok24
                    LD ABCD, 0xFFFF8000
                    ADD16 ABCD, 0x8000      ; ADDSUB__rrrr_n - 16-bit
                    JP Z, .Ok25
                    LD X, 25
                    BREAK
                .Ok25
                    LD ABCD, 0xFF800000
                    ADD24 ABCD, 0x800000    ; ADDSUB__rrrr_n - 24 bit
                    JP Z, .Ok26
                    LD X, 26
                    BREAK
                .Ok26
                    LD ABCD, 0x80000000
                    ADD32 ABCD, 0x80000000  ; ADDSUB__rrrr_n - 32 bit
                    JP Z, .Ok27
                    LD X, 27
                    BREAK
                .Ok27
                    LD ABCD, 0xFFFFFFFF
                    LD E, 1
                    ADD ABCD, E             ; ADDSUB_rrrr_r
                    JP Z, .Ok28
                    LD X, 28
                    BREAK
                .Ok28
                    LD ABCD, 0xFFFF8000
                    LD EF, 0x8000
                    ADD ABCD, EF            ; ADDSUB_rrrr_rr
                    JP Z, .Ok29
                    LD X, 29
                    BREAK
                .Ok29
                    LD ABCD, 0xFF800000
                    LD EFG, 0x800000
                    ADD ABCD, EFG           ; ADDSUB_rrrr_rrr
                    JP Z, .Ok30
                    LD X, 30
                    BREAK
                .Ok30
                    LD ABCD, 0x80000000
                    LD EFGH, 0x80000000
                    ADD ABCD, EFGH          ; ADDSUB_rrrr_rrrr
                    JP Z, .Ok31
                    LD X, 31
                    BREAK
                .Ok31
                    LD ABCD, 0xFFFFFFFF
                    ADD ABCD, (10000)       ; ADDSUB__rrrr_InnnI - 8 bit
                    JP Z, .Ok32
                    LD X, 32
                    BREAK
                .Ok32
                    LD ABCD, 0xFFFF8000
                    ADD16 ABCD, (10015)      ; ADDSUB__rrrr_InnnI - 16-bit
                    JP Z, .Ok33
                    LD X, 33
                    BREAK
                .Ok33
                    LD ABCD, 0xFF800000
                    ADD24 ABCD, (10025)    ; ADDSUB__rrrr_InnnI - 24 bit
                    JP Z, .Ok34
                    LD X, 34
                    BREAK
                .Ok34
                    LD ABCD, 0x80000000
                    ADD32 ABCD, (10035)  ; ADDSUB__rrrr_InnnI - 32 bit
                    JP Z, .Ok35
                    LD X, 35
                    BREAK
                .Ok35
                    LD ABCD, 0xFFFFFFFF
                    LD EFG, 10000
                    ADD ABCD, (EFG)         ; ADDSUB_rrrr_IrrrI
                    JP Z, .Ok36
                    LD X, 36
                    BREAK
                .Ok36
                    LD ABCD, 0xFFFF8000
                    LD EFG, 10015
                    ADD16 ABCD, (EFG)       ; ADDSUB16_rrrr_IrrrI
                    JP Z, .Ok37
                    LD X, 37
                    BREAK
                .Ok37
                    LD ABCD, 0xFF800000
                    LD EFG, 10025
                    ADD24 ABCD, (EFG)       ; ADDSUB24_rrrr_IrrrI
                    JP Z, .Ok38
                    LD X, 38
                    BREAK
                .Ok38
                    LD ABCD, 0x80000000
                    LD EFG, 10035
                    ADD32 ABCD, (EFG)       ; ADDSUB32_rrrr_IrrrI
                    JP Z, .Ok39
                    LD X, 39
                    BREAK
                .Ok39
                    LD (20000), 0x80, 1
                    ADD (20000), 0x80       ; ADDSUB__InnnI_nnn - 8 bit
                    JP Z, .Ok40
                    LD X, 40
                    BREAK
                .Ok40
                    LD (20000), 0x8000, 2
                    ADD16 (20000), 0x8000   ; ADDSUB__InnnI_nnn - 16-bit
                    JP Z, .Ok41
                    LD X, 41
                    BREAK
                .Ok41
                    LD (20000), 0x800000, 3
                    ADD24 (20000), 0x800000 ; ADDSUB__InnnI_nnn - 24 bit
                    JP Z, .Ok42
                    LD X, 42
                    BREAK
                .Ok42
                    LD (20000), 0x80000000, 4
                    ADD32 (20000), 0x80000000 ; ADDSUB__InnnI_nnn - 32 bit
                    JP Z, .Ok43
                    LD X, 43
                    BREAK
                .Ok43
                    LD (20000), 0x80
                    LD A, 0x80
                    ADD (20000), A          ; ADDSUB__InnnI_r - 8 bit
                    JP Z, .Ok44
                    LD X, 44
                    BREAK
                .Ok44
                    LD (20000), 0xFF80, 2
                    LD A, 0x80
                    ADD16 (20000), A          ; ADDSUB__InnnI_r - 16-bit
                    JP Z, .Ok45
                    LD X, 45
                    BREAK
                .Ok45
                    LD (20000), 0xFFFF80, 3
                    LD A, 0x80
                    ADD24 (20000), A          ; ADDSUB__InnnI_r - 24 bit
                    JP Z, .Ok46
                    LD X, 46
                    BREAK
                .Ok46
                    LD (20000), 0xFFFFFF80, 4
                    LD A, 0x80
                    ADD32 (20000), A          ; ADDSUB__InnnI_r - 32 bit
                    JP Z, .Ok47
                    LD X, 47
                    BREAK
                .Ok47
                    LD (20000), 0x8000, 2
                    LD AB, 0x8000
                    ADD16 (20000), AB         ; ADDSUB__InnnI_rr - 16-bit
                    JP Z, .Ok48
                    LD X, 48
                    BREAK
                .Ok48
                    LD (20000), 0xFF8000, 3
                    LD AB, 0x8000
                    ADD24 (20000), AB         ; ADDSUB__InnnI_rr - 24 bit
                    JP Z, .Ok49
                    LD X, 49
                    BREAK
                .Ok49
                    LD (20000), 0xFFFF8000, 4
                    LD AB, 0x8000
                    ADD32 (20000), AB         ; ADDSUB__InnnI_rr - 32 bit
                    JP Z, .Ok50
                    LD X, 50
                    BREAK
                .Ok50
                    LD (20000), 0x800000, 3
                    LD ABC, 0x800000
                    ADD24 (20000), ABC        ; ADDSUB__InnnI_rrr - 24 bit
                    JP Z, .Ok51
                    LD X, 51
                    BREAK
                .Ok51
                    LD (20000), 0xFF800000, 4
                    LD ABC, 0x800000
                    ADD32 (20000), ABC        ; ADDSUB__InnnI_rrr - 32 bit
                    JP Z, .Ok52
                    LD X, 52
                    BREAK
                .Ok52
                    LD (20000), 0x80000000, 4
                    LD ABCD, 0x80000000
                    ADD (20000), ABCD       ; ADDSUB__InnnI_rrrr - 32 bit
                    JP Z, .Ok53
                    LD X, 53
                    BREAK
                .Ok53
                    LD ABC, 20000
                    LD (ABC), 0x80
                    ADD (ABC), 0x80         ; ADDSUB__IrrrI_nnn - 8 bit
                    JP Z, .Ok54
                    LD X, 54
                    BREAK
                .Ok54
                    LD ABC, 20000
                    LD (ABC), 0x8000, 2
                    ADD16 (ABC), 0x8000         ; ADDSUB__IrrrI_nnn - 16-bit
                    JP Z, .Ok55
                    LD X, 55
                    BREAK
                .Ok55
                    LD ABC, 20000
                    LD (ABC), 0x800000, 3
                    ADD24 (ABC), 0x800000       ; ADDSUB__IrrrI_nnn - 24 bit
                    JP Z, .Ok56
                    LD X, 56
                    BREAK
                .Ok56
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    ADD32 (ABC), 0x80000000     ; ADDSUB__IrrrI_nnn - 32 bit
                    JP Z, .Ok57
                    LD X, 57
                    BREAK
                .Ok57
                    LD ABC, 20000
                    LD D, 0x80
                    LD (ABC), 0x80
                    ADD (ABC), D            ; ADDSUB__IrrrI_r - 8 bit
                    JP Z, .Ok58
                    LD X, 58
                    BREAK
                .Ok58
                    LD ABC, 20000
                    LD D, 0x80
                    LD (ABC), 0xFF80, 2
                    ADD16 (ABC), D          ; ADDSUB__IrrrI_r - 16-bit
                    JP Z, .Ok59
                    LD X, 59
                    BREAK
                .Ok59
                    LD ABC, 20000
                    LD D, 0x80
                    LD (ABC), 0xFFFF80, 3
                    ADD24 (ABC), D          ; ADDSUB__IrrrI_r - 24 bit
                    JP Z, .Ok60
                    LD X, 60
                    BREAK
                .Ok60
                    LD ABC, 20000
                    LD D, 0x80
                    LD (ABC), 0xFFFFFF80, 4
                    ADD32 (ABC), D          ; ADDSUB__IrrrI_r - 32 bit
                    JP Z, .Ok61
                    LD X, 61
                    BREAK
                .Ok61
                    LD ABC, 20000
                    LD DE, 0x8000
                    LD (ABC), 0x8000, 2
                    ADD16 (ABC), DE         ; ADDSUB__IrrrI_rr - 16-bit
                    JP Z, .Ok62
                    LD X, 62
                    BREAK
                .Ok62
                    LD ABC, 20000
                    LD DE, 0x8000
                    LD (ABC), 0xFF8000, 3
                    ADD24 (ABC), DE         ; ADDSUB__IrrrI_rr - 24 bit
                    JP Z, .Ok63
                    LD X, 63
                    BREAK
                .Ok63
                    LD ABC, 20000
                    LD DE, 0x8000
                    LD (ABC), 0xFFFF8000, 4
                    ADD32 (ABC), DE         ; ADDSUB__IrrrI_rr - 32 bit
                    JP Z, .Ok64
                    LD X, 64
                    BREAK
                .Ok64
                    LD ABC, 20000
                    LD DEF, 0x800000
                    LD (ABC), 0x800000, 3
                    ADD24 (ABC), DEF        ; ADDSUB__IrrrI_rrr - 24 bit
                    JP Z, .Ok65
                    LD X, 65
                    BREAK
                .Ok65
                    LD ABC, 20000
                    LD DEF, 0x800000
                    LD (ABC), 0xFF800000, 4
                    ADD32 (ABC), DEF        ; ADDSUB__IrrrI_rrr - 32 bit
                    JP Z, .Ok66
                    LD X, 66
                    BREAK
                .Ok66
                    LD ABC, 20000
                    LD DEFG, 0x80000000
                    LD (ABC), 0x80000000, 4
                    ADD (ABC), DEFG         ; ADDSUB__IrrrI_rrrr - 32 bit
                    JP Z, .Ok67
                    LD X, 67
                    BREAK
                .Ok67
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // In case of error, X contains asigned value before which the condition failed
            Assert.Equal(0, computer.CPU.REGS.X);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestFLAG_CARRY_ADD()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.MEMC.Set8bitToRAM(10000, 0x1);
            computer.MEMC.Set8bitToRAM(10005, 0x80);
            computer.MEMC.Set16bitToRAM(10010, 0x1);
            computer.MEMC.Set16bitToRAM(10015, 0x8000);
            computer.MEMC.Set24bitToRAM(10020, 0x1);
            computer.MEMC.Set24bitToRAM(10025, 0x800000);
            computer.MEMC.Set32bitToRAM(10030, 0x1);
            computer.MEMC.Set32bitToRAM(10035, 0x80000000);

            computer.CPU.REGS.X = 0;


            /*
                This is a string of direct assembly tests.
             */
            cp.Build(@"
                    LD A,0x80
                    ADD A, 0x1
                    JP NC, .OkNotCarry1
                    LD X, 1
                    BREAK
                .OkNotCarry1
                    LD A,0x80
                    ADD A, 0xA0
                    JP C, .OkCarry1
                    LD X, 2
                    BREAK
                .OkCarry1
                    LD A,0x80
                    LD B,1
                    ADD A, B
                    JP NC, .OkNotCarry2
                    LD X, 3
                    BREAK
                .OkNotCarry2
                    LD A,0x80
                    LD B,0xA0
                    ADD A, B
                    JP C, .OkCarry2
                    LD X, 4
                    BREAK
                .OkCarry2
                    LD A,0xA0
                    ADD A, (10000)
                    JP NC, .OkNotCarry3
                    LD X, 5
                    BREAK
                .OkNotCarry3
                    LD A,0xA0
                    ADD A, (10005)
                    JP C, .OkCarry3
                    LD X, 6
                    BREAK
                .OkCarry3
                    LD A,0xA0
                    LD BCD, 10000
                    ADD A, (BCD)
                    JP NC, .OkNotCarry4
                    LD X, 7
                    BREAK
                .OkNotCarry4
                    LD A,0xA0
                    LD BCD, 10005
                    ADD A, (BCD)
                    JP C, .OkCarry4
                    LD X, 8
                    BREAK
                .OkCarry4
                    LD AB,0xFFA0
                    ADD AB, 1
                    JP NC, .OkNotCarry5
                    LD X, 9
                    BREAK
                .OkNotCarry5
                    LD AB,0xFFA0
                    ADD AB, 0x80
                    JP C, .OkCarry5
                    LD X, 10
                    BREAK
                .OkCarry5
                    LD AB,0xAFA0
                    ADD16 AB, 0x1000
                    JP NC, .OkNotCarry6
                    LD X, 11
                    BREAK
                .OkNotCarry6
                    LD AB,0xFFA0
                    ADD16 AB, 0x8000
                    JP C, .OkCarry6
                    LD X, 12
                    BREAK
                .OkCarry6
                    LD AB,0xAFA0
                    LD CD, 0x1000
                    ADD AB, CD
                    JP NC, .OkNotCarry7
                    LD X, 13
                    BREAK
                .OkNotCarry7
                    LD AB,0xFFA0
                    LD CD, 0xD000
                    ADD AB, CD
                    JP C, .OkCarry7
                    LD X, 14
                    BREAK
                .OkCarry7
                    LD AB,0xAFA0
                    ADD AB, (10005)
                    JP NC, .OkNotCarry8
                    LD X, 15
                    BREAK
                .OkNotCarry8
                    LD AB,0xFFFA
                    ADD AB, (10005)
                    JP C, .OkCarry8
                    LD X, 16
                    BREAK
                .OkCarry8
                    LD AB,0x1FA0
                    ADD16 AB, (10015)
                    JP NC, .OkNotCarry9
                    LD X, 17
                    BREAK
                .OkNotCarry9
                    LD AB,0xFFFA
                    ADD16 AB, (10015)
                    JP C, .OkCarry9
                    LD X, 18
                    BREAK
                .OkCarry9
                    LD AB,0x1FA0
                    LD CDE, 10005
                    ADD AB, (CDE)
                    JP NC, .OkNotCarry10
                    LD X, 19
                    BREAK
                .OkNotCarry10
                    LD AB,0xFFFA
                    LD CDE, 10005
                    ADD AB, (CDE)
                    JP C, .OkCarry10
                    LD X, 20
                    BREAK
                .OkCarry10
                    LD AB,0x1FA0
                    LD CDE, 10015
                    ADD16 AB, (CDE)
                    JP NC, .OkNotCarry11
                    LD X, 21
                    BREAK
                .OkNotCarry11
                    LD AB,0xFFFA
                    LD CDE, 10015
                    ADD16 AB, (CDE)
                    JP C, .OkCarry11
                    LD X, 22
                    BREAK
                .OkCarry11
                    LD ABC,0x1FA000
                    ADD ABC, 0xAA
                    JP NC, .OkNotCarry12
                    LD X, 23
                    BREAK
                .OkNotCarry12
                    LD ABC,0xFFFFFA
                    ADD ABC, 0xAA
                    JP C, .OkCarry12
                    LD X, 24
                    BREAK
                .OkCarry12
                    LD ABC,0x1FA000
                    ADD16 ABC, 0x1000
                    JP NC, .OkNotCarry13
                    LD X, 25
                    BREAK
                .OkNotCarry13
                    LD ABC,0xFFFFFA
                    ADD16 ABC, 0xAAAA
                    JP C, .OkCarry13
                    LD X, 26
                    BREAK
                .OkCarry13  ; ADDSUB__rrr_n
                    LD ABC,0x1FA000
                    ADD24 ABC, 0x100000
                    JP NC, .OkNotCarry14
                    LD X, 27
                    BREAK
                .OkNotCarry14
                    LD ABC,0xFFFFFA
                    ADD24 ABC, 0xAAAAAA
                    JP C, .OkCarry14
                    LD X, 28
                    BREAK
                .OkCarry14  ; ADDSUB_rrr_r
                    LD ABC,0x1FA000
                    LD D, 0x10
                    ADD ABC, D
                    JP NC, .OkNotCarry15
                    LD X, 29
                    BREAK
                .OkNotCarry15
                    LD ABC,0xFFFFFA
                    LD D, 0xA0
                    ADD ABC, D
                    JP C, .OkCarry15
                    LD X, 30
                    BREAK
                .OkCarry15  ; ADDSUB_rrr_rr
                    LD ABC,0x1FA000
                    LD DE, 0x1000
                    ADD ABC, DE
                    JP NC, .OkNotCarry16
                    LD X, 31
                    BREAK
                .OkNotCarry16
                    LD ABC,0xFFFFFA
                    LD DE, 0xA000
                    ADD ABC, DE
                    JP C, .OkCarry16
                    LD X, 32
                    BREAK
                .OkCarry16              ; ADDSUB_rrr_rrr
                    LD ABC, 0x1FA000
                    LD DEF, 0x100000
                    ADD ABC, DEF
                    JP NC, .OkNotCarry17
                    LD X, 33
                    BREAK
                .OkNotCarry17
                    LD ABC, 0xFFFFFA
                    LD DEF, 0xA00000
                    ADD ABC, DEF
                    JP C, .OkCarry17
                    LD X, 34
                    BREAK
                .OkCarry17              ; ADDSUB__rrr_InnnI - 8 bit
                    LD ABC, 0x1FA000
                    ADD ABC, (10005)
                    JP NC, .OkNotCarry18
                    LD X, 35
                    BREAK
                .OkNotCarry18
                    LD ABC, 0xFFFFFA
                    ADD ABC, (10005)
                    JP C, .OkCarry18
                    LD X, 36
                    BREAK
                .OkCarry18              ; ADDSUB__rrr_InnnI - 16-bit
                    LD ABC, 0x1FA000
                    ADD16 ABC, (10015)
                    JP NC, .OkNotCarry19
                    LD X, 37
                    BREAK
                .OkNotCarry19
                    LD ABC, 0xFFFFFA
                    ADD16 ABC, (10015)
                    JP C, .OkCarry19
                    LD X, 38
                    BREAK
                .OkCarry19              ; ADDSUB__rrr_InnnI - 24 bit
                    LD ABC, 0x1FA000
                    ADD24 ABC, (10025)
                    JP NC, .OkNotCarry20
                    LD X, 39
                    BREAK
                .OkNotCarry20
                    LD ABC, 0xFFFFFA
                    ADD24 ABC, (10025)
                    JP C, .OkCarry20
                    LD X, 40
                    BREAK
                .OkCarry20              ; ADDSUB_rrr_IrrrI
                    LD ABC, 0x1FA000
                    LD DEF, 10005
                    ADD ABC, (DEF)
                    JP NC, .OkNotCarry21
                    LD X, 41
                    BREAK
                .OkNotCarry21
                    LD ABC, 0xFFFFFA
                    LD DEF, 10005
                    ADD ABC, (DEF)
                    JP C, .OkCarry21
                    LD X, 42
                    BREAK
                .OkCarry21              ; ADDSUB16_rrr_IrrrI
                    LD ABC, 0x1FA000
                    LD DEF, 10015
                    ADD16 ABC, (DEF)
                    JP NC, .OkNotCarry22
                    LD X, 43
                    BREAK
                .OkNotCarry22
                    LD ABC, 0xFFFFFA
                    LD DEF, 10015
                    ADD16 ABC, (DEF)
                    JP C, .OkCarry22
                    LD X, 44
                    BREAK
                .OkCarry22              ; ADDSUB24_rrr_IrrrI
                    LD ABC, 0x1FA000
                    LD DEF, 10025
                    ADD24 ABC, (DEF)
                    JP NC, .OkNotCarry23
                    LD X, 45
                    BREAK
                .OkNotCarry23
                    LD ABC, 0xFFFFFA
                    LD DEF, 10025
                    ADD24 ABC, (DEF)
                    JP C, .OkCarry23
                    LD X, 46
                    BREAK
                .OkCarry23              ; ADDSUB__rrrr_n - 8 bit
                    LD ABCD, 0x1FA00000
                    ADD ABCD, 0x10
                    JP NC, .OkNotCarry24
                    LD X, 47
                    BREAK
                .OkNotCarry24
                    LD ABCD, 0xFFFFFFAA
                    ADD ABCD, 0xAA
                    JP C, .OkCarry24
                    LD X, 48
                    BREAK
                .OkCarry24              ; ADDSUB__rrrr_n - 16-bit
                    LD ABCD, 0x1FA00000
                    ADD16 ABCD, 0x1000
                    JP NC, .OkNotCarry25
                    LD X, 49
                    BREAK
                .OkNotCarry25
                    LD ABCD, 0xFFFFFFAA
                    ADD16 ABCD, 0xAAAA
                    JP C, .OkCarry25
                    LD X, 50
                    BREAK
                .OkCarry25              ; ADDSUB__rrrr_n - 24 bit
                    LD ABCD, 0x1FA00000
                    ADD24 ABCD, 0x100000
                    JP NC, .OkNotCarry26
                    LD X, 51
                    BREAK
                .OkNotCarry26
                    LD ABCD, 0xFFFFFFAA
                    ADD24 ABCD, 0xAAAAAA
                    JP C, .OkCarry26
                    LD X, 52
                    BREAK
                .OkCarry26              ; ADDSUB__rrrr_n - 32 bit
                    LD ABCD, 0x1FA00000
                    ADD32 ABCD, 0x10000000
                    JP NC, .OkNotCarry27
                    LD X, 53
                    BREAK
                .OkNotCarry27
                    LD ABCD, 0xFFFFFFAA
                    ADD32 ABCD, 0xAAAAAAAA
                    JP C, .OkCarry27
                    LD X, 54
                    BREAK
                .OkCarry27              ; ADDSUB_rrrr_r
                    LD ABCD, 0x1FA00000
                    LD E, 0x10
                    ADD ABCD, E
                    JP NC, .OkNotCarry28
                    LD X, 55
                    BREAK
                .OkNotCarry28
                    LD ABCD, 0xFFFFFFAA
                    LD E, 0xAA
                    ADD ABCD, E
                    JP C, .OkCarry28
                    LD X, 56
                    BREAK
                .OkCarry28              ; ADDSUB_rrrr_rr
                    LD ABCD, 0x1FA00000
                    LD EF, 0x1000
                    ADD ABCD, EF
                    JP NC, .OkNotCarry29
                    LD X, 57
                    BREAK
                .OkNotCarry29
                    LD ABCD, 0xFFFFFFAA
                    LD EF, 0xAAAA
                    ADD ABCD, EF
                    JP C, .OkCarry29
                    LD X, 58
                    BREAK
                .OkCarry29              ; ADDSUB_rrrr_rrr
                    LD ABCD, 0x1FA00000
                    LD EFG, 0x100000
                    ADD ABCD, EFG
                    JP NC, .OkNotCarry30
                    LD X, 59
                    BREAK
                .OkNotCarry30
                    LD ABCD, 0xFFFFFFAA
                    LD EFG, 0xAAAAAA
                    ADD ABCD, EFG
                    JP C, .OkCarry30
                    LD X, 60
                    BREAK
                .OkCarry30              ; ADDSUB_rrrr_rrrr
                    LD ABCD, 0x1FA00000
                    LD EFGH, 0x10000000
                    ADD ABCD, EFGH
                    JP NC, .OkNotCarry31
                    LD X, 61
                    BREAK
                .OkNotCarry31
                    LD ABCD, 0xFFFFFFAA
                    LD EFGH, 0xAAAAAAAA
                    ADD ABCD, EFGH
                    JP C, .OkCarry31
                    LD X, 62
                    BREAK
                .OkCarry31              ; ADDSUB__rrrr_InnnI - 8 bit
                    LD ABCD, 0x1FA00000
                    ADD ABCD, (10005)
                    JP NC, .OkNotCarry32
                    LD X, 63
                    BREAK
                .OkNotCarry32
                    LD ABCD, 0xFFFFFFAA
                    ADD ABCD, (10005)
                    JP C, .OkCarry32
                    LD X, 64
                    BREAK
                .OkCarry32              ; ADDSUB__rrrr_InnnI - 16-bit
                    LD ABCD, 0x1FA00000
                    ADD16 ABCD, (10015)
                    JP NC, .OkNotCarry33
                    LD X, 65
                    BREAK
                .OkNotCarry33
                    LD ABCD, 0xFFFFFFAA
                    ADD16 ABCD, (10015)
                    JP C, .OkCarry33
                    LD X, 66
                    BREAK
                .OkCarry33              ; ADDSUB__rrrr_InnnI - 24 bit
                    LD ABCD, 0x1FA00000
                    ADD24 ABCD, (10025)
                    JP NC, .OkNotCarry34
                    LD X, 67
                    BREAK
                .OkNotCarry34
                    LD ABCD, 0xFFFFFFAA
                    ADD24 ABCD, (10025)
                    JP C, .OkCarry34
                    LD X, 68
                    BREAK
                .OkCarry34              ; ADDSUB__rrrr_InnnI - 32 bit
                    LD ABCD, 0x1FA00000
                    ADD32 ABCD, (10035)
                    JP NC, .OkNotCarry35
                    LD X, 69
                    BREAK
                .OkNotCarry35
                    LD ABCD, 0xFFFFFFAA
                    ADD32 ABCD, (10035)
                    JP C, .OkCarry35
                    LD X, 70
                    BREAK
                .OkCarry35              ; ADDSUB_rrrr_IrrrI
                    LD ABCD, 0x1FA00000
                    LD EFG, 10005
                    ADD ABCD, (EFG)
                    JP NC, .OkNotCarry36
                    LD X, 71
                    BREAK
                .OkNotCarry36
                    LD ABCD, 0xFFFFFFAA
                    LD EFG, 10005
                    ADD ABCD, (EFG)
                    JP C, .OkCarry36
                    LD X, 72
                    BREAK
                .OkCarry36              ; ADDSUB16_rrrr_IrrrI
                    LD ABCD, 0x1FA00000
                    LD EFG, 10015
                    ADD16 ABCD, (EFG)
                    JP NC, .OkNotCarry37
                    LD X, 73
                    BREAK
                .OkNotCarry37
                    LD ABCD, 0xFFFFFFAA
                    LD EFG, 10015
                    ADD16 ABCD, (EFG)
                    JP C, .OkCarry37
                    LD X, 74
                    BREAK
                .OkCarry37              ; ADDSUB24_rrrr_IrrrI
                    LD ABCD, 0x1FA00000
                    LD EFG, 10025
                    ADD24 ABCD, (EFG)
                    JP NC, .OkNotCarry38
                    LD X, 75
                    BREAK
                .OkNotCarry38
                    LD ABCD, 0xFFFFFFAA
                    LD EFG, 10025
                    ADD24 ABCD, (EFG)
                    JP C, .OkCarry38
                    LD X, 76
                    BREAK
                .OkCarry38              ; ADDSUB32_rrrr_IrrrI
                    LD ABCD, 0x1FA00000
                    LD EFG, 10035
                    ADD32 ABCD, (EFG)
                    JP NC, .OkNotCarry39
                    LD X, 77
                    BREAK
                .OkNotCarry39
                    LD ABCD, 0xFFFFFFAA
                    LD EFG, 10035
                    ADD32 ABCD, (EFG)
                    JP C, .OkCarry39
                    LD X, 78
                    BREAK
                .OkCarry39              ; ADDSUB__InnnI_nnn - 8 bit
                    LD (20000), 0xAA, 1
                    ADD (20000), 0x10
                    JP NC, .OkNotCarry40
                    LD X, 79
                    BREAK
                .OkNotCarry40
                    LD (20000), 0xAA, 1
                    ADD (20000), 0xAA
                    JP C, .OkCarry40
                    LD X, 80
                    BREAK
                .OkCarry40              ; ADDSUB__InnnI_nnn - 16-bit
                    LD (20000), 0xA000, 2
                    ADD16 (20000), 0x1000
                    JP NC, .OkNotCarry41
                    LD X, 81
                    BREAK
                .OkNotCarry41
                    LD (20000), 0xAAAA, 2
                    ADD16 (20000), 0xAAAA
                    JP C, .OkCarry41
                    LD X, 82
                    BREAK
                .OkCarry41              ; ADDSUB__InnnI_nnn - 24 bit
                    LD (20000), 0xA00000, 3
                    ADD24 (20000), 0x100000
                    JP NC, .OkNotCarry42
                    LD X, 83
                    BREAK
                .OkNotCarry42
                    LD (20000), 0xAAAAAA, 3
                    ADD24 (20000), 0xAAAAAA
                    JP C, .OkCarry42
                    LD X, 84
                    BREAK
                .OkCarry42              ; ADDSUB__InnnI_nnn - 32 bit
                    LD (20000), 0xA0000000, 4
                    ADD32 (20000), 0x10000000
                    JP NC, .OkNotCarry43
                    LD X, 85
                    BREAK
                .OkNotCarry43
                    LD (20000), 0xAAAAAAAA, 4
                    ADD32 (20000), 0xAAAAAAAA
                    JP C, .OkCarry43
                    LD X, 86
                    BREAK
                .OkCarry43              ; ADDSUB__InnnI_r - 8 bit
                    LD (20000), 0xA0, 1
                    LD A, 0x10
                    ADD (20000), A
                    JP NC, .OkNotCarry44
                    LD X, 87
                    BREAK
                .OkNotCarry44
                    LD (20000), 0xAA, 1
                    LD A, 0xAA
                    ADD (20000), A
                    JP C, .OkCarry44
                    LD X, 88
                    BREAK
                .OkCarry44              ; ADDSUB__InnnI_r - 16-bit
                    LD (20000), 0xA000, 2
                    LD A, 0x10
                    ADD16 (20000), A
                    JP NC, .OkNotCarry45
                    LD X, 89
                    BREAK
                .OkNotCarry45
                    LD (20000), 0xFFFF, 2
                    LD A, 0xAA
                    ADD16 (20000), A
                    JP C, .OkCarry45
                    LD X, 90
                    BREAK
                .OkCarry45              ; ADDSUB__InnnI_r - 24 bit
                    LD (20000), 0xA00000, 3
                    LD A, 0x10
                    ADD24 (20000), A
                    JP NC, .OkNotCarry46
                    LD X, 91
                    BREAK
                .OkNotCarry46
                    LD (20000), 0xFFFFFF, 3
                    LD A, 0xAA
                    ADD24 (20000), A
                    JP C, .OkCarry46
                    LD X, 92
                    BREAK
                .OkCarry46              ; ADDSUB__InnnI_r - 32 bit
                    LD (20000), 0xA0000000, 4
                    LD A, 0x10
                    ADD32 (20000), A
                    JP NC, .OkNotCarry47
                    LD X, 93
                    BREAK
                .OkNotCarry47
                    LD (20000), 0xFFFFFFFF, 4
                    LD A, 0xAA
                    ADD32 (20000), A
                    JP C, .OkCarry47
                    LD X, 94
                    BREAK
                .OkCarry47              ; ADDSUB__InnnI_rr - 16-bit
                    LD (20000), 0xA000, 2
                    LD AB, 0x1000
                    ADD16 (20000), AB
                    JP NC, .OkNotCarry48
                    LD X, 95
                    BREAK
                .OkNotCarry48
                    LD (20000), 0xF000, 2
                    LD AB, 0xAAAA
                    ADD16 (20000), AB
                    JP C, .OkCarry48
                    LD X, 96
                    BREAK
                .OkCarry48              ; ADDSUB__InnnI_rr - 24 bit
                    LD (20000), 0xFFA000, 3
                    LD AB, 0x1000
                    ADD24 (20000), AB
                    JP NC, .OkNotCarry49
                    LD X, 97
                    BREAK
                .OkNotCarry49
                    LD (20000), 0xFFF000, 3
                    LD AB, 0xAAAA
                    ADD24 (20000), AB
                    JP C, .OkCarry49
                    LD X, 98
                    BREAK
                .OkCarry49              ; ADDSUB__InnnI_rr - 32 bit
                    LD (20000), 0xFFFFA000, 4
                    LD AB, 0x1000
                    ADD32 (20000), AB
                    JP NC, .OkNotCarry50
                    LD X, 99
                    BREAK
                .OkNotCarry50
                    LD (20000), 0xFFFFF000, 4
                    LD AB, 0xAAAA
                    ADD32 (20000), AB
                    JP C, .OkCarry50
                    LD X, 100
                    BREAK
                .OkCarry50              ; ADDSUB__InnnI_rrr - 24 bit
                    LD (20000), 0xA00000, 3
                    LD ABC, 0x100000
                    ADD24 (20000), ABC
                    JP NC, .OkNotCarry51
                    LD X, 101
                    BREAK
                .OkNotCarry51
                    LD (20000), 0xFFF000, 3
                    LD ABC, 0xAAAAAA
                    ADD24 (20000), ABC
                    JP C, .OkCarry51
                    LD X, 102
                    BREAK
                .OkCarry51              ; ADDSUB__InnnI_rrr - 32 bit
                    LD (20000), 0xFFA00000, 3
                    LD ABC, 0x100000
                    ADD24 (20000), ABC
                    JP NC, .OkNotCarry52
                    LD X, 103
                    BREAK
                .OkNotCarry52
                    LD (20000), 0xFFFFF000, 3
                    LD ABC, 0xAAAAAA
                    ADD24 (20000), ABC
                    JP C, .OkCarry52
                    LD X, 104
                    BREAK
                .OkCarry52              ; ADDSUB__InnnI_rrrr - 32 bit
                    LD (20000), 0xA0000000, 4
                    LD ABCD, 0x10000000
                    ADD (20000), ABCD
                    JP NC, .OkNotCarry53
                    LD X, 105
                    BREAK
                .OkNotCarry53
                    LD (20000), 0xFFFFF000, 4
                    LD ABCD, 0xAAAAAAAA
                    ADD (20000), ABCD
                    JP C, .OkCarry53
                    LD X, 106
                    BREAK
                .OkCarry53              ; ADDSUB__IrrrI_nnn - 8 bit
                    LD ABC, 20000
                    LD (ABC), 0x80
                    ADD (ABC), 0x10
                    JP NC, .OkNotCarry54
                    LD X, 107
                    BREAK
                .OkNotCarry54
                    LD ABC, 20000
                    LD (ABC), 0xFF
                    ADD (ABC), 0x80
                    JP C, .OkCarry54
                    LD X, 108
                    BREAK
                .OkCarry54              ; ADDSUB__IrrrI_nnn - 16-bit
                    LD ABC, 20000
                    LD (ABC), 0x8000, 2
                    ADD16 (ABC), 0x1000
                    JP NC, .OkNotCarry55
                    LD X, 109
                    BREAK
                .OkNotCarry55
                    LD ABC, 20000
                    LD (ABC), 0xFFFF, 2
                    ADD16 (ABC), 0x8000
                    JP C, .OkCarry55
                    LD X, 110
                    BREAK
                .OkCarry55              ; ADDSUB__IrrrI_nnn - 24 bit
                    LD ABC, 20000
                    LD (ABC), 0x800000, 3
                    ADD24 (ABC), 0x100000
                    JP NC, .OkNotCarry56
                    LD X, 111
                    BREAK
                .OkNotCarry56
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFF, 3
                    ADD24 (ABC), 0x800000
                    JP C, .OkCarry56
                    LD X, 112
                    BREAK
                .OkCarry56              ; ADDSUB__IrrrI_nnn - 32 bit
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    ADD32 (ABC), 0x10000000
                    JP NC, .OkNotCarry57
                    LD X, 113
                    BREAK
                .OkNotCarry57
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFFFF, 4
                    ADD32 (ABC), 0x80000000
                    JP C, .OkCarry57
                    LD X, 114
                    BREAK
                .OkCarry57              ; ADDSUB__IrrrI_r - 8 bit
                    LD ABC, 20000
                    LD (ABC), 0x80
                    LD D, 0x10
                    ADD (ABC), D
                    JP NC, .OkNotCarry58
                    LD X, 115
                    BREAK
                .OkNotCarry58
                    LD ABC, 20000
                    LD (ABC), 0xFF
                    LD D, 0x80
                    ADD (ABC), D
                    JP C, .OkCarry58
                    LD X, 116
                    BREAK
                .OkCarry58              ; ADDSUB__IrrrI_r - 16-bit
                    LD ABC, 20000
                    LD (ABC), 0x8000, 2
                    LD D, 0x10
                    ADD16 (ABC), D
                    JP NC, .OkNotCarry59
                    LD X, 117
                    BREAK
                .OkNotCarry59
                    LD ABC, 20000
                    LD (ABC), 0xFFFF, 2
                    LD D, 0x80
                    ADD16 (ABC), D
                    JP C, .OkCarry59
                    LD X, 118
                    BREAK
                .OkCarry59              ; ADDSUB__IrrrI_r - 24 bit
                    LD ABC, 20000
                    LD (ABC), 0x800000, 3
                    LD D, 0x10
                    ADD24 (ABC), D
                    JP NC, .OkNotCarry60
                    LD X, 119
                    BREAK
                .OkNotCarry60
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFF, 3
                    LD D, 0x80
                    ADD24 (ABC), D
                    JP C, .OkCarry60
                    LD X, 120
                    BREAK
                .OkCarry60              ; ADDSUB__IrrrI_r - 32 bit
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    LD D, 0x10
                    ADD32 (ABC), D
                    JP NC, .OkNotCarry61
                    LD X, 121
                    BREAK
                .OkNotCarry61
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFFFF, 4
                    LD D, 0x80
                    ADD32 (ABC), D
                    JP C, .OkCarry61
                    LD X, 122
                    BREAK
                .OkCarry61              ; ADDSUB__IrrrI_rr - 16-bit
                    LD ABC, 20000
                    LD (ABC), 0x8000, 2
                    LD DE, 0x1000
                    ADD16 (ABC), DE
                    JP NC, .OkNotCarry62
                    LD X, 123
                    BREAK
                .OkNotCarry62
                    LD ABC, 20000
                    LD (ABC), 0xFFFF, 2
                    LD DE, 0x8000
                    ADD16 (ABC), DE
                    JP C, .OkCarry62
                    LD X, 124
                    BREAK
                .OkCarry62              ; ADDSUB__IrrrI_rr - 24 bit
                    LD ABC, 20000
                    LD (ABC), 0x800000, 3
                    LD DE, 0x1000
                    ADD24 (ABC), DE
                    JP NC, .OkNotCarry63
                    LD X, 125
                    BREAK
                .OkNotCarry63
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFF, 3
                    LD DE, 0x8000
                    ADD24 (ABC), DE
                    JP C, .OkCarry63
                    LD X, 126
                    BREAK
                .OkCarry63              ; ADDSUB__IrrrI_rr - 32 bit
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    LD DE, 0x1000
                    ADD32 (ABC), DE
                    JP NC, .OkNotCarry64
                    LD X, 127
                    BREAK
                .OkNotCarry64
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFFFF, 4
                    LD DE, 0x8000
                    ADD32 (ABC), DE
                    JP C, .OkCarry64
                    LD X, 128
                    BREAK
                .OkCarry64              ; ADDSUB__IrrrI_rrr - 24 bit
                    LD ABC, 20000
                    LD (ABC), 0x800000, 3
                    LD DEF, 0x100000
                    ADD24 (ABC), DEF
                    JP NC, .OkNotCarry65
                    LD X, 129
                    BREAK
                .OkNotCarry65
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFF, 3
                    LD DEF, 0x800000
                    ADD24 (ABC), DEF
                    JP C, .OkCarry65
                    LD X, 130
                    BREAK
                .OkCarry65              ; ADDSUB__IrrrI_rrr - 32 bit
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    LD DEF, 0x100000
                    ADD32 (ABC), DEF
                    JP NC, .OkNotCarry66
                    LD X, 131
                    BREAK
                .OkNotCarry66
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFFFF, 4
                    LD DEF, 0x800000
                    ADD32 (ABC), DEF
                    JP C, .OkCarry66
                    LD X, 132
                    BREAK
                .OkCarry66              ; ADDSUB__IrrrI_rrrr - 32 bit
                    LD ABC, 20000
                    LD (ABC), 0x80000000, 4
                    LD DEFG, 0x10000000
                    ADD (ABC), DEFG
                    JP NC, .OkNotCarry67
                    LD X, 133
                    BREAK
                .OkNotCarry67
                    LD ABC, 20000
                    LD (ABC), 0xFFFFFFFF, 4
                    LD DEFG, 0x800000
                    ADD (ABC), DEFG
                    JP C, .OkCarry67
                    LD X, 134
                    BREAK
                .OkCarry67

                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // In case of error, X contains asigned value before which the condition failed
            Assert.Equal(0, computer.CPU.REGS.X);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
