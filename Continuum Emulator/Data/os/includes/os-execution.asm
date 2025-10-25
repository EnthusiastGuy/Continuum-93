; Takes the path to a valid *.asm file, compiles and loads it in memory
; then starts it by calling the start address

.CompileAndExecuteASMFile
        PUSH A, Z
        ;CALLR .ClearAllLayers       ; Clear all video pages to offer a clean slate
        CALLR .SetVideoAutoControlMode  ; REset this for applications that want to start in auto mode
        
        LD ABC, .CurrentFileFullPath
        LD Z, 0xC0
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

.ClearMessage
        LD (.ErrorsFoundCompiling), 0, 1
        LD AB, 0x0200	; Video page and color to clear
		CALLR .ClearScreen
        CALLR .ListRecords
        CALLR .DisplayRecordInfo
        RET
