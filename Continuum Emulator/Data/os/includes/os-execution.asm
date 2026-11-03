; Takes the path to a valid *.asm or *.c file, compiles and loads it in memory
; then starts it by calling the start address

.CompileAndExecuteASMFile
        PUSH A, Z
        ;CALLR .ClearAllLayers       ; Clear all video pages to offer a clean slate
        CALLR .SetVideoAutoControlMode  ; REset this for applications that want to start in auto mode

        LD Z, 0xC0                  ; Build (assembly)
        CALLR .IsCSourceFile
        CP Y, 1
        JR NZ, .compile_selected_file
        LD Z, 0xC2                  ; Build C
.compile_selected_file
        LD ABC, .CurrentFileFullPath
		INT 0, Z                    ; Compile target program
        CP D, 0
        CALLR NZ, .HandleCompileError
        JR NZ, .RecoverOS

        

        CALL ABC                    ; Call target program

.RecoverOS
        ; Recover OS state
        CALLR .InitVideo
        CALLR .ListRecords			; Initial records display
		CALLR .SelectionBar			; Initial selection bar display
		CALLR .DisplayRecordInfo	; Initial display of the record info
        POP A, Z
		RET

.HandleCompileError
        LD (.ErrorsFoundCompiling), D
        RET

; Checks whether .CurrentFileFullPath ends in ".c" or ".C"
; output: Y = 1 if it does, 0 otherwise (uses MNO, P)
.IsCSourceFile
        LD Y, 0
        LD MNO, .CurrentFileFullPath
.is_c_find_end
        CP (MNO), 0
        JR Z, .is_c_found_end
        INC MNO
        JR .is_c_find_end
.is_c_found_end
        SUB MNO, 2
        LD P, (MNO)
        CP P, 0x2E                  ; '.'
        JR NZ, .is_c_done
        INC MNO
        LD P, (MNO)
        OR P, 0x20                  ; lower case
        CP P, 0x63                  ; 'c'
        JR NZ, .is_c_done
        LD Y, 1
.is_c_done
        RET

.ClearMessage
        LD (.ErrorsFoundCompiling), 0, 1
        LD AB, 0x0200	; Video page and color to clear
		CALLR .ClearScreen
        CALLR .ListRecords
        CALLR .DisplayRecordInfo
        RET
